from dataclasses import dataclass

import jwt
from jwt import PyJWKClient

from .config import Settings


class TokenValidationError(ValueError):
    pass


@dataclass(frozen=True)
class Caller:
    object_id: str
    tenant_id: str


class AccessTokenValidator:
    def __init__(self, settings: Settings) -> None:
        self._settings = settings
        self._issuer = (
            f"https://login.microsoftonline.com/{settings.entra_tenant_id}/v2.0"
        )
        self._jwks = PyJWKClient(
            f"https://login.microsoftonline.com/{settings.entra_tenant_id}/discovery/v2.0/keys"
        )

    def validate(self, token: str) -> Caller:
        try:
            signing_key = self._jwks.get_signing_key_from_jwt(token)
            claims = jwt.decode(
                token,
                signing_key.key,
                algorithms=["RS256"],
                audience=[
                    self._settings.broker_client_id,
                    f"api://{self._settings.broker_client_id}",
                ],
                issuer=self._issuer,
                options={"require": ["aud", "exp", "iss", "tid", "oid"]},
            )
        except jwt.PyJWTError as exc:
            raise TokenValidationError("The bearer token is invalid.") from exc

        if claims["tid"] != self._settings.entra_tenant_id:
            raise TokenValidationError("The bearer token was issued by another tenant.")

        scopes = set(str(claims.get("scp", "")).split())
        if self._settings.broker_required_scope not in scopes:
            raise TokenValidationError("The bearer token lacks the required delegated scope.")

        return Caller(object_id=claims["oid"], tenant_id=claims["tid"])