import json
from threading import Lock
from typing import Any

from azure.ai.projects import AIProjectClient
from azure.ai.projects.models import FunctionTool, PromptAgentDefinition
from azure.identity import DefaultAzureCredential
from openai.types.responses.response_input_param import FunctionCallOutput

from .case_api import CaseApiClient, CaseApiError
from .config import Settings


class FoundryAgentService:
    def __init__(self, settings: Settings) -> None:
        self._settings = settings
        self._project = AIProjectClient(
            endpoint=str(settings.foundry_project_endpoint),
            credential=DefaultAzureCredential(),
        )
        self._openai = self._project.get_openai_client()
        self._agent_version: Any | None = None
        self._agent_lock = Lock()

    def ask(self, prompt: str, user_assertion: str) -> str:
        agent = self._get_or_create_agent()
        conversation = self._openai.conversations.create()
        case_api = CaseApiClient(self._settings, user_assertion)

        try:
            response = self._openai.responses.create(
                input=prompt,
                conversation=conversation.id,
                extra_body={
                    "agent_reference": {
                        "name": agent.name,
                        "version": agent.version,
                        "type": "agent_reference",
                    }
                },
            )

            for _ in range(8):
                tool_outputs: list[FunctionCallOutput] = []
                for item in response.output:
                    if item.type != "function_call":
                        continue
                    output = self._execute_tool(
                        case_api, item.name, json.loads(item.arguments)
                    )
                    tool_outputs.append(
                        FunctionCallOutput(
                            type="function_call_output",
                            call_id=item.call_id,
                            output=json.dumps(output),
                        )
                    )

                if not tool_outputs:
                    return response.output_text

                response = self._openai.responses.create(
                    input=tool_outputs,
                    conversation=conversation.id,
                    extra_body={
                        "agent_reference": {
                            "name": agent.name,
                            "version": agent.version,
                            "type": "agent_reference",
                        }
                    },
                )

            raise RuntimeError("The agent exceeded the maximum tool-call depth.")
        finally:
            self._openai.conversations.delete(conversation_id=conversation.id)

    def _get_or_create_agent(self) -> Any:
        if self._agent_version is not None:
            return self._agent_version

        with self._agent_lock:
            if self._agent_version is None:
                self._agent_version = self._project.agents.create_version(
                    agent_name=self._settings.foundry_agent_name,
                    definition=PromptAgentDefinition(
                        model=self._settings.foundry_model_deployment,
                        instructions=(
                            "You are OGE Refining Case Agent. Answer questions only from "
                            "the read-only Case API tools. Search before retrieving details "
                            "when the case reference is unknown. State when the API has no "
                            "matching data, and never invent case facts."
                        ),
                        tools=self._tools(),
                    ),
                )

        return self._agent_version

    @staticmethod
    def _execute_tool(
        case_api: CaseApiClient, name: str, arguments: dict[str, Any]
    ) -> dict[str, Any]:
        try:
            if name == "list_cases":
                return case_api.list_cases(**arguments)
            if name == "get_case":
                return case_api.get_case(arguments["reference"])
            if name == "get_current_caller":
                return case_api.get_current_caller()
            return {"error": "Unsupported tool call."}
        except CaseApiError as exc:
            return {"error": exc.detail, "statusCode": exc.status_code}

    @staticmethod
    def _tools() -> list[FunctionTool]:
        return [
            FunctionTool(
                name="list_cases",
                description="Search and filter refinery case summaries.",
                parameters={
                    "type": "object",
                    "properties": {
                        "search": {"type": "string"},
                        "status": {
                            "type": "string",
                            "enum": ["Open", "InProgress", "OnHold", "Closed"],
                        },
                        "severity": {
                            "type": "string",
                            "enum": [
                                "Informational",
                                "Minor",
                                "Moderate",
                                "Major",
                                "Critical",
                            ],
                        },
                        "type": {
                            "type": "string",
                            "enum": [
                                "Incident",
                                "NearMiss",
                                "NonConformance",
                                "SafetyObservation",
                                "MaintenanceIssue",
                                "EnvironmentalConcern",
                            ],
                        },
                        "page": {"type": "integer", "minimum": 1},
                        "pageSize": {
                            "type": "integer",
                            "minimum": 1,
                            "maximum": 100,
                        },
                    },
                    "additionalProperties": False,
                },
                strict=False,
            ),
            FunctionTool(
                name="get_case",
                description="Get a case and timeline by case number or ID.",
                parameters={
                    "type": "object",
                    "properties": {
                        "reference": {
                            "type": "string",
                            "description": "A case number such as SYN-2026-0009 or case ID.",
                        }
                    },
                    "required": ["reference"],
                    "additionalProperties": False,
                },
                strict=True,
            ),
            FunctionTool(
                name="get_current_caller",
                description="Resolve the signed-in user through the Case API's Graph OBO call.",
                parameters={
                    "type": "object",
                    "properties": {},
                    "additionalProperties": False,
                },
                strict=True,
            ),
        ]