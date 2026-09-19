import { PublicClientApplication, type Configuration, type RedirectRequest } from '@azure/msal-browser'

const requireEnvironmentVariable = (name: string, value: string | undefined) => {
  if (!value) {
    throw new Error(`${name} is required. Copy .env.example to .env and configure it.`)
  }

  return value
}

const tenantId = requireEnvironmentVariable('VITE_ENTRA_TENANT_ID', import.meta.env.VITE_ENTRA_TENANT_ID)
const clientId = requireEnvironmentVariable('VITE_ENTRA_CLIENT_ID', import.meta.env.VITE_ENTRA_CLIENT_ID)
export const apiScope = requireEnvironmentVariable('VITE_API_SCOPE', import.meta.env.VITE_API_SCOPE)

const configuration: Configuration = {
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
}

export const loginRequest: RedirectRequest = {
  scopes: [apiScope],
}

export const msalInstance = new PublicClientApplication(configuration)