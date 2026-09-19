import { type FormEvent, useEffect, useRef, useState } from 'react'
import { useMsal } from '@azure/msal-react'
import { InteractionRequiredAuthError } from '@azure/msal-browser'
import { AlertCircle, Bot, LoaderCircle, RotateCcw, Send, UserRound, X } from 'lucide-react'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import { askCaseBuddy, brokerScope } from './agent'

interface FoundryBuddyProps {
  onClose: () => void
}

interface ChatMessage {
  id: number
  role: 'assistant' | 'user'
  text: string
}

const welcomeMessage: ChatMessage = {
  id: 0,
  role: 'assistant',
  text: 'What would you like to know about your cases?',
}

export default function FoundryBuddy({ onClose }: FoundryBuddyProps) {
  const { instance, accounts } = useMsal()
  const account = instance.getActiveAccount() ?? accounts[0]
  const accountHomeId = account.homeAccountId
  const requestController = useRef<AbortController | null>(null)
  const messageList = useRef<HTMLDivElement>(null)
  const nextMessageId = useRef(1)
  const [messages, setMessages] = useState<ChatMessage[]>([welcomeMessage])
  const [draft, setDraft] = useState('')
  const [sending, setSending] = useState(false)
  const [authorizationRequired, setAuthorizationRequired] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    messageList.current?.scrollTo({ top: messageList.current.scrollHeight, behavior: 'smooth' })
  }, [messages, sending])

  useEffect(() => () => requestController.current?.abort(), [])

  async function authorize() {
    setError('')
    try {
      const currentAccount = instance.getAllAccounts().find((item) => item.homeAccountId === accountHomeId)
      await instance.acquireTokenPopup({ account: currentAccount, scopes: [brokerScope] })
      setAuthorizationRequired(false)
    } catch (authorizationError) {
      const detail = authorizationError instanceof Error ? authorizationError.message : String(authorizationError)
      setAuthorizationRequired(false)
      setError(`Foundry Agent authorization failed. ${detail.slice(0, 240)}`)
    }
  }

  async function sendMessage(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const message = draft.trim()
    if (!message || sending) return

    setMessages((current) => [...current, { id: nextMessageId.current++, role: 'user', text: message }])
    setDraft('')
    setError('')
    setSending(true)

    try {
      const currentAccount = instance.getAllAccounts().find((item) => item.homeAccountId === accountHomeId)
      const token = await instance.acquireTokenSilent({ account: currentAccount, scopes: [brokerScope] })
      requestController.current = new AbortController()
      const answer = await askCaseBuddy(message, token.accessToken, requestController.current.signal)
      setMessages((current) => [...current, { id: nextMessageId.current++, role: 'assistant', text: answer }])
    } catch (requestError) {
      if (requestError instanceof InteractionRequiredAuthError) {
        setAuthorizationRequired(true)
      } else if ((requestError as Error).name !== 'AbortError') {
        const detail = requestError instanceof Error ? requestError.message : String(requestError)
        setError(detail.slice(0, 240))
      }
    } finally {
      requestController.current = null
      setSending(false)
    }
  }

  function resetConversation() {
    requestController.current?.abort()
    setMessages([welcomeMessage])
    setDraft('')
    setError('')
    setSending(false)
  }

  return <aside className="buddy-panel foundry-panel" aria-label="Foundry Case Agent">
    <header className="buddy-header">
      <div className="buddy-title"><span><Bot size={18} /></span><div><strong>Foundry Case Agent</strong><small>Microsoft Foundry</small></div></div>
      <div className="buddy-actions">
        <button type="button" onClick={resetConversation} title="Start a new conversation"><RotateCcw size={18} /></button>
        <button type="button" onClick={onClose} title="Close Foundry Agent"><X size={20} /></button>
      </div>
    </header>
    <div className="buddy-content">
      <div className="buddy-messages" ref={messageList} aria-live="polite">
        {messages.map((message) => <article className={`buddy-message ${message.role}`} key={message.id}>
          <span>{message.role === 'assistant' ? <Bot size={17} /> : <UserRound size={17} />}</span>
          {message.role === 'assistant'
            ? <div className="buddy-markdown"><ReactMarkdown remarkPlugins={[remarkGfm]}>{message.text}</ReactMarkdown></div>
            : <p>{message.text}</p>}
        </article>)}
        {sending && <div className="buddy-thinking"><LoaderCircle size={17} /><span>Reviewing case data...</span></div>}
      </div>
      {authorizationRequired && <div className="buddy-notice" role="alert"><AlertCircle size={18} /><div><p>Microsoft authorization is required for the Foundry Agent.</p><button type="button" onClick={authorize}>Continue with Microsoft</button></div></div>}
      {error && <div className="buddy-notice" role="alert"><AlertCircle size={18} /><p>{error}</p></div>}
      <form className="buddy-composer" onSubmit={sendMessage}>
        <label className="sr-only" htmlFor="foundry-message">Message Foundry Agent</label>
        <textarea id="foundry-message" value={draft} onChange={(event) => setDraft(event.target.value)} placeholder="Ask about cases..." rows={2} maxLength={8000} disabled={sending} onKeyDown={(event) => {
          if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault()
            event.currentTarget.form?.requestSubmit()
          }
        }} />
        <button type="submit" title="Send message" disabled={!draft.trim() || sending}><Send size={19} /></button>
      </form>
    </div>
  </aside>
}