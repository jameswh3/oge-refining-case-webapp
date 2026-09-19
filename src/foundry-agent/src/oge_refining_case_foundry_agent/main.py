from typing import Annotated

import uvicorn
from fastapi import Depends, FastAPI, HTTPException, status
from fastapi.middleware.cors import CORSMiddleware
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer
from pydantic import BaseModel, Field

from .agent import FoundryAgentService
from .auth import AccessTokenValidator, TokenValidationError
from .config import Settings, get_settings


class ChatRequest(BaseModel):
    message: str = Field(min_length=1, max_length=8000)


class ChatResponse(BaseModel):
    answer: str


bearer_scheme = HTTPBearer(auto_error=False)


def create_app(settings: Settings | None = None) -> FastAPI:
    resolved_settings = settings or get_settings()
    token_validator = AccessTokenValidator(resolved_settings)
    agent_service = FoundryAgentService(resolved_settings)
    app = FastAPI(title="OGE Refining Case Foundry Agent", version="0.1.0")
    if resolved_settings.allowed_origins:
        app.add_middleware(
            CORSMiddleware,
            allow_origins=resolved_settings.allowed_origins,
            allow_methods=["POST"],
            allow_headers=["Authorization", "Content-Type"],
        )

    @app.get("/health")
    def health() -> dict[str, str]:
        return {"status": "healthy"}

    @app.post("/api/chat", response_model=ChatResponse)
    def chat(
        request: ChatRequest,
        credentials: Annotated[
            HTTPAuthorizationCredentials | None, Depends(bearer_scheme)
        ],
    ) -> ChatResponse:
        if credentials is None or credentials.scheme.lower() != "bearer":
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail="A delegated bearer token is required.",
            )

        try:
            token_validator.validate(credentials.credentials)
        except TokenValidationError as exc:
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED, detail=str(exc)
            ) from exc

        answer = agent_service.ask(request.message, credentials.credentials)
        return ChatResponse(answer=answer)

    return app


def run() -> None:
    uvicorn.run(
        "oge_refining_case_foundry_agent.main:create_app",
        factory=True,
        host="127.0.0.1",
        port=8000,
    )


if __name__ == "__main__":
    run()