from typing import Any

import httpx
from azure.identity import ManagedIdentityCredential, OnBehalfOfCredential

from .config import Settings


class CaseApiError(RuntimeError):
    def __init__(self, status_code: int, detail: str) -> None:
        super().__init__(detail)
        self.status_code = status_code
        self.detail = detail


class CaseApiClient:
    def __init__(self, settings: Settings, user_assertion: str) -> None:
        self._settings = settings
        self._user_assertion = user_assertion

    def list_cases(self, **filters: Any) -> dict[str, Any]:
        params = {key: value for key, value in filters.items() if value is not None}
        return self._get("/api/v1/cases", params=params)

    def get_case(self, reference: str) -> dict[str, Any]:
        return self._get(f"/api/v1/cases/{reference}")

    def get_current_caller(self) -> dict[str, Any]:
        return self._get("/api/v1/me")

    def _get(
        self, path: str, params: dict[str, Any] | None = None
    ) -> dict[str, Any]:
        access_token = self._get_obo_token()

        with httpx.Client(
            base_url=str(self._settings.case_api_base_url), timeout=20.0
        ) as client:
            response = client.get(
                path,
                params=params,
                headers={"Authorization": f"Bearer {access_token.token}"},
            )

        if response.is_error:
            detail = "The Case API request failed."
            if response.headers.get("content-type", "").startswith("application/problem+json"):
                detail = response.json().get("detail", detail)
            raise CaseApiError(response.status_code, detail)

        return response.json()

    def _get_obo_token(self) -> Any:
        if self._settings.broker_client_secret is not None:
            with OnBehalfOfCredential(
                tenant_id=self._settings.entra_tenant_id,
                client_id=self._settings.broker_client_id,
                client_secret=(
                    self._settings.broker_client_secret.get_secret_value()
                ),
                user_assertion=self._user_assertion,
            ) as credential:
                return credential.get_token(self._settings.case_api_scope)

        with ManagedIdentityCredential(
            client_id=self._settings.broker_managed_identity_client_id
        ) as managed_identity:
            with OnBehalfOfCredential(
                tenant_id=self._settings.entra_tenant_id,
                client_id=self._settings.broker_client_id,
                client_assertion_func=lambda: managed_identity.get_token(
                    "api://AzureADTokenExchange/.default"
                ).token,
                user_assertion=self._user_assertion,
            ) as credential:
                return credential.get_token(self._settings.case_api_scope)