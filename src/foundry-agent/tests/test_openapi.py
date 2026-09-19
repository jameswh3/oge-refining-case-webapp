import json
from pathlib import Path


SPEC_PATH = Path(__file__).parents[1] / "openapi" / "case-api.openapi.json"


def test_openapi_contract_targets_only_customer_rest_routes() -> None:
    spec = json.loads(SPEC_PATH.read_text(encoding="utf-8"))

    assert spec["openapi"].startswith("3.")
    assert spec["servers"] == [
        {"url": "https://your-case-api.example.com/api/v1"}
    ]
    assert set(spec["paths"]) == {"/cases", "/cases/{reference}", "/me"}
    assert "/mcp" not in json.dumps(spec).lower()


def test_openapi_operations_and_bearer_auth_are_stable() -> None:
    spec = json.loads(SPEC_PATH.read_text(encoding="utf-8"))
    operation_ids = {
        operation["operationId"]
        for path in spec["paths"].values()
        for operation in path.values()
    }
    assert operation_ids == {"ListCases", "GetCase", "GetCurrentCaller"}
    assert spec["security"] == [{"bearerAuth": []}]
    assert spec["components"]["securitySchemes"]["bearerAuth"] == {
        "type": "http",
        "scheme": "bearer",
        "bearerFormat": "JWT",
    }