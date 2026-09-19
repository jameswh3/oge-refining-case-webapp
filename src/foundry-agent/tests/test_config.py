import pytest
from pydantic import ValidationError

from oge_refining_case_foundry_agent.config import Settings


def test_exactly_one_confidential_client_method_is_required() -> None:
    common = {
        "entra_tenant_id": "tenant-id",
        "broker_client_id": "broker-client-id",
        "case_api_base_url": "https://case-api.example",
        "case_api_scope": "api://case-api/case.read",
        "foundry_project_endpoint": "https://foundry.example/api/projects/test",
        "foundry_model_deployment": "test-model",
    }

    with pytest.raises(ValidationError):
        Settings(_env_file=None, **common)

    settings = Settings(
        _env_file=None,
        broker_client_secret="local-only-secret",
        cors_allowed_origins="http://localhost:5173, https://case.example/",
        **common,
    )
    assert settings.broker_client_secret is not None
    assert settings.allowed_origins == [
        "http://localhost:5173",
        "https://case.example",
    ]