import { LogIn } from 'lucide-react'
import { useIsAuthenticated, useMsal } from '@azure/msal-react'
import App from './App'
import { loginRequest } from './auth'

export default function Root() {
  const isAuthenticated = useIsAuthenticated()
  const { instance } = useMsal()

  if (isAuthenticated) return <App />

  return <main className="sign-in-page">
    <section className="sign-in-panel">
      <span>Northstar Demonstration Refinery</span>
      <h1>Case reporting</h1>
      <p>Sign in with your lab tenant account to access delegated refinery case reports.</p>
      <button type="button" onClick={() => instance.loginRedirect(loginRequest)}>
        <LogIn size={18} /> Sign in with Microsoft Entra
      </button>
    </section>
  </main>
}