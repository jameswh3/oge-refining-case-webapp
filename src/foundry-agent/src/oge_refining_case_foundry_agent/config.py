from functools import lru_cache

from pydantic import HttpUrl, SecretStr, model_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore",
        frozen=True,
    )

    entra_tenant_id: str
    broker_client_id: str
    broker_client_secret: SecretStr | None = None
    broker_managed_identity_client_id: str | None = None
    broker_required_scope: str = "agent.invoke"
    cors_allowed_origins: str = ""

    case_api_base_url: HttpUrl
    case_api_scope: str

    foundry_project_endpoint: HttpUrl
    foundry_model_deployment: str
    foundry_agent_name: str = "oge-refining-case-agent"

    @property
    def allowed_origins(self) -> list[str]:
        return [
            origin.strip().rstrip("/")
            for origin in self.cors_allowed_origins.split(",")
            if origin.strip()
        ]

    @model_validator(mode="after")
    def validate_confidential_client(self) -> "Settings":
        methods = (
            self.broker_client_secret is not None,
            self.broker_managed_identity_client_id is not None,
        )
        if sum(methods) != 1:
            raise ValueError(
                "Set exactly one of BROKER_CLIENT_SECRET or "
                "BROKER_MANAGED_IDENTITY_CLIENT_ID."
            )
        return self


@lru_cache
def get_settings() -> Settings:
    return Settings()  # pyright: ignore[reportCallIssue]