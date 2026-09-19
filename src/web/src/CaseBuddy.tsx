import { createElement, useEffect, useRef, useState } from 'react'
import { createRoot, type Root } from 'react-dom/client'
import { useMsal } from '@azure/msal-react'
import { InteractionRequiredAuthError } from '@azure/msal-browser'
import type { CopilotStudioWebChatConnection } from '@microsoft/agents-copilotstudio-client'
import { AlertCircle, Bot, LoaderCircle, RotateCcw, X } from 'lucide-react'
import { copilotConfiguration, copilotScope } from './copilot'

interface CaseBuddyProps {
  onClose: () => void
}

export default function CaseBuddy({ onClose }: CaseBuddyProps) {
  const { instance, accounts } = useMsal()
  const account = instance.getActiveAccount() ?? accounts[0]
  const accountHomeId = account.homeAccountId
  const accountLocalId = account.localAccountId
  const accountUsername = account.username
  const accountDisplayName = account.name ?? accountUsername
  const chatHost = useRef<HTMLDivElement>(null)
  const [conversationKey, setConversationKey] = useState(0)
  const [connected, setConnected] = useState(false)
  const [authorizationRequired, setAuthorizationRequired] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    let connection: CopilotStudioWebChatConnection | undefined
    let webChatRoot: Root | undefined
    let cancelled = false

    async function connect() {
      setError('')
      setAuthorizationRequired(false)
      setConnected(false)
      try {
        const [{ ConnectionSettings, CopilotStudioClient, CopilotStudioWebChat }, { ReactWebChat }] = await Promise.all([
          import('@microsoft/agents-copilotstudio-client'),
          import('botframework-webchat'),
        ])
        const currentAccount = instance.getAllAccounts().find((item) => item.homeAccountId === accountHomeId)
        const token = await instance.acquireTokenSilent({ account: currentAccount, scopes: [copilotScope] })
        if (cancelled || !chatHost.current) return

        const client = new CopilotStudioClient(new ConnectionSettings(copilotConfiguration), token.accessToken)
        connection = CopilotStudioWebChat.createConnection(client, { showTyping: true })
        webChatRoot = createRoot(chatHost.current)
        webChatRoot.render(createElement(ReactWebChat, {
          directLine: connection,
          userID: accountLocalId,
          username: accountDisplayName,
          locale: 'en-US',
          styleOptions: {
            accent: '#086f68',
            backgroundColor: '#f5f7f6',
            botAvatarBackgroundColor: '#086f68',
            botAvatarInitials: 'CB',
            bubbleBackground: '#ffffff',
            bubbleBorderColor: '#d9dfdc',
            bubbleBorderRadius: 4,
            bubbleFromUserBackground: '#dcefe5',
            bubbleFromUserBorderColor: '#b8d8cc',
            bubbleFromUserBorderRadius: 4,
            hideUploadButton: true,
            primaryFont: 'Aptos, sans-serif',
            sendBoxBackground: '#ffffff',
            sendBoxButtonColor: '#086f68',
          },
        }))
        setConnected(true)
      } catch (connectError) {
        if (!cancelled) {
          console.error('Case Buddy connection failed.', connectError)
          if (connectError instanceof InteractionRequiredAuthError) {
            setAuthorizationRequired(true)
          } else {
            const detail = connectError instanceof Error ? connectError.message : String(connectError)
            setError(`Case Buddy could not connect. ${detail.slice(0, 240)}`)
          }
        }
      }
    }

    void connect()
    return () => {
      cancelled = true
      webChatRoot?.unmount()
      connection?.end()
    }
  }, [accountDisplayName, accountHomeId, accountLocalId, conversationKey, instance])

  async function authorize() {
    setError('')
    try {
      const currentAccount = instance.getAllAccounts().find((item) => item.homeAccountId === accountHomeId)
      await instance.acquireTokenPopup({ account: currentAccount, scopes: [copilotScope] })
      setConversationKey((key) => key + 1)
    } catch (authorizationError) {
      const detail = authorizationError instanceof Error ? authorizationError.message : String(authorizationError)
      setAuthorizationRequired(false)
      setError(`Case Buddy authorization failed. ${detail.slice(0, 240)}`)
    }
  }

  return <aside className="buddy-panel copilot-panel" aria-label="Refining Case Buddy">
    <header className="buddy-header">
      <div className="buddy-title"><span><Bot size={18} /></span><div><strong>Refining Case Buddy</strong><small>Copilot Studio</small></div></div>
      <div className="buddy-actions">
        <button type="button" onClick={() => setConversationKey((key) => key + 1)} title="Start a new conversation"><RotateCcw size={18} /></button>
        <button type="button" onClick={onClose} title="Close Case Buddy"><X size={20} /></button>
      </div>
    </header>
    <div className="buddy-content">
      {!connected && !error && <div className="buddy-loading"><LoaderCircle size={20} /><span>Connecting to Case Buddy...</span></div>}
      {authorizationRequired && <div className="buddy-error" role="alert"><AlertCircle size={20} /><div><p>Case Buddy needs a Power Platform sign-in for this session.</p><button type="button" onClick={authorize}>Authorize Case Buddy</button></div></div>}
      {error && <div className="buddy-error" role="alert"><AlertCircle size={20} /><p>{error}</p></div>}
      <div className="buddy-webchat" ref={chatHost} />
    </div>
  </aside>
}