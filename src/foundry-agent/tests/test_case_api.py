from types import SimpleNamespace

import respx
from httpx import Response

from oge_refining_case_foundry_agent import case_api
from oge_refining_case_foundry_agent.case_api import CaseApiClient
from oge_refining_case_foundry_agent.config import Settings


def create_settings() -> Settings:
    return Settings(
        _env_file=None,
        entra_tenant_id="tenant-id",
        broker_client_id="broker-client-id",
        broker_client_secret="local-only-secret",
        case_api_base_url="https://case-api.example",
        case_api_scope="api://case-api/case.read",
        foundry_project_endpoint="https://foundry.example/api/projects/test",
        foundry_model_deployment="test-model",
    )


def test_list_cases_exchanges_assertion_for_case_api_token(monkeypatch) -> None:
    captured: dict[str, str] = {}

    class FakeOboCredential:
        def __init__(self, **kwargs: str) -> None:
            captured.update(kwargs)

        def __enter__(self) -> "FakeOboCredential":
            return self

        def __exit__(self, *args: object) -> None:
            return None

        def get_token(self, scope: str) -> SimpleNamespace:
            captured["scope"] = scope
            return SimpleNamespace(token="exchanged-case-token")

    monkeypatch.setattr(case_api, "OnBehalfOfCredential", FakeOboCredential)
    settings = create_settings()

    with respx.mock:
        route = respx.get(
            "https://case-api.example/api/v1/cases",
            params={"search": "pump"},
        ).mock(return_value=Response(200, json={"items": [], "totalCount": 0}))
        result = CaseApiClient(settings, "incoming-user-token").list_cases(
            search="pump"
        )

    assert result["totalCount"] == 0
    assert captured["user_assertion"] == "incoming-user-token"
    assert captured["scope"] == settings.case_api_scope
    assert route.calls[0].request.headers["Authorization"] == (
        "Bearer exchanged-case-token"
    )