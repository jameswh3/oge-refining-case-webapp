import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { MsalProvider } from '@azure/msal-react'
import './index.css'
import { msalInstance } from './auth'
import Root from './Root'

async function bootstrap() {
  await msalInstance.initialize()
  const redirectResult = await msalInstance.handleRedirectPromise()
  if (redirectResult?.account) msalInstance.setActiveAccount(redirectResult.account)
  if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
    msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0])
  }

  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <MsalProvider instance={msalInstance}>
        <Root />
      </MsalProvider>
    </StrictMode>,
  )
}

void bootstrap()
