import { useEffect, useState } from 'react'
import { Activity, AlertTriangle, Bot, ChevronRight, CircleDot, Factory, FileText, MessageSquare, Search, ShieldCheck, X } from 'lucide-react'
import { useMsal } from '@azure/msal-react'
import { getCase, listCases } from './api'
import { apiScope } from './auth'
import type { CaseDetail, CaseSeverity, CaseStatus, CaseSummary } from './types'
import CaseBuddy from './CaseBuddy'
import FoundryBuddy from './FoundryBuddy'
import './App.css'

const statuses: Array<'' | CaseStatus> = ['', 'Open', 'InProgress', 'OnHold', 'Closed']
const severities: Array<'' | CaseSeverity> = ['', 'Critical', 'Major', 'Moderate', 'Minor', 'Informational']
const formatLabel = (value: string) => value.replace(/([a-z])([A-Z])/g, '$1 $2')
const formatDate = (value: string) => new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric', year: 'numeric' }).format(new Date(value))

function App() {
  const { instance, accounts } = useMsal()
  const account = instance.getActiveAccount() ?? accounts[0]
  const accountHomeId = account.homeAccountId
  const [cases, setCases] = useState<CaseSummary[]>([])
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<'' | CaseStatus>('')
  const [severity, setSeverity] = useState<'' | CaseSeverity>('')
  const [selectedCase, setSelectedCase] = useState<CaseDetail | null>(null)
  const [buddyOpen, setBuddyOpen] = useState(false)
  const [foundryOpen, setFoundryOpen] = useState(false)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const controller = new AbortController()
    const currentAccount = instance.getAllAccounts().find((item) => item.homeAccountId === accountHomeId)
    setLoading(true)
    setError('')
    instance.acquireTokenSilent({ account: currentAccount, scopes: [apiScope] })
      .then((token) => listCases({ search, status, severity }, token.accessToken, controller.signal))
      .then((result) => setCases(result.items))
      .catch((requestError: unknown) => {
        if ((requestError as Error).name !== 'AbortError') setError('Case data is unavailable. Confirm the API is running on port 5016.')
      })
      .finally(() => setLoading(false))
    return () => controller.abort()
  }, [accountHomeId, instance, search, status, severity])

  async function openCase(item: CaseSummary) {
    setError('')
    try {
      const token = await instance.acquireTokenSilent({ account, scopes: [apiScope] })
      setSelectedCase(await getCase(item.id, token.accessToken))
    }
    catch { setError('The selected case could not be loaded.') }
  }

  const openCount = cases.filter((item) => item.status !== 'Closed').length
  const criticalCount = cases.filter((item) => item.severity === 'Critical').length

  return <div className="app-shell">
    <header className="topbar">
      <div className="brand-mark" aria-hidden="true"><Factory size={22} /></div>
      <div className="brand-copy"><strong>Northstar Refining</strong><span>Case intelligence</span></div>
      <div className="environment"><CircleDot size={14} /> Lab environment</div>
      <button className="profile" type="button" onClick={() => instance.logoutRedirect({ account })} title="Sign out"><span>{(account.name ?? account.username).split(/\s|@/).slice(0, 2).map((part) => part[0]).join('').toUpperCase()}</span><span className="profile-copy">{account.name ?? 'Lab user'}<small>{account.username}</small></span></button>
    </header>

    <aside className="sidebar" aria-label="Primary navigation">
      <button className="nav-item active" type="button" title="Cases"><FileText size={20} /><span>Cases</span></button>
      <button className="nav-item" type="button" title="Operational overview"><Activity size={20} /><span>Overview</span></button>
      <button className="nav-item" type="button" title="Compliance"><ShieldCheck size={20} /><span>Compliance</span></button>
      <button className={`nav-item ${buddyOpen ? 'active' : ''}`} type="button" title="Refining Case Buddy (Copilot Studio)" onClick={() => { setSelectedCase(null); setFoundryOpen(false); setBuddyOpen((open) => !open) }}><Bot size={20} /><span>Case Buddy</span></button>
      <button className={`nav-item ${foundryOpen ? 'active' : ''}`} type="button" title="Foundry Case Agent" onClick={() => { setSelectedCase(null); setBuddyOpen(false); setFoundryOpen((open) => !open) }}><MessageSquare size={20} /><span>Foundry</span></button>
    </aside>

    <main>
      <div className="page-heading"><div><p className="eyebrow">Northstar Demonstration Refinery</p><h1>Case reporting</h1></div><p className="synthetic-note"><AlertTriangle size={16} /> Synthetic lab data only</p></div>
      <section className="metrics" aria-label="Current case summary">
        <article><span>Visible cases</span><strong>{cases.length}</strong><small>Current filter set</small></article>
        <article><span>Active review</span><strong>{openCount}</strong><small>Excludes closed cases</small></article>
        <article className="metric-alert"><span>Critical severity</span><strong>{criticalCount}</strong><small>Requires attention</small></article>
      </section>

      <section className="workspace" aria-labelledby="case-list-title">
        <div className="workspace-header"><div><h2 id="case-list-title">Refinery cases</h2><p>Search and inspect reported operational events.</p></div><span className="result-count">{cases.length} results</span></div>
        <div className="filters">
          <label className="search-field"><Search size={18} /><span className="sr-only">Search cases</span><input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search case, title, or unit" /></label>
          <label><span className="sr-only">Filter by status</span><select value={status} onChange={(event) => setStatus(event.target.value as '' | CaseStatus)}>{statuses.map((item) => <option key={item || 'all'} value={item}>{item ? formatLabel(item) : 'All statuses'}</option>)}</select></label>
          <label><span className="sr-only">Filter by severity</span><select value={severity} onChange={(event) => setSeverity(event.target.value as '' | CaseSeverity)}>{severities.map((item) => <option key={item || 'all'} value={item}>{item || 'All severities'}</option>)}</select></label>
        </div>
        {error && <div className="error-banner" role="alert">{error}</div>}
        <div className="table-wrap"><table>
          <thead><tr><th>Case</th><th>Process unit</th><th>Type</th><th>Status</th><th>Severity</th><th>Reported</th><th><span className="sr-only">Open</span></th></tr></thead>
          <tbody>{loading ? <tr><td colSpan={7} className="empty-state">Loading case records...</td></tr> : cases.length === 0 ? <tr><td colSpan={7} className="empty-state">No cases match these filters.</td></tr> : cases.map((item) => <tr key={item.id} onClick={() => openCase(item)}>
            <td><button className="case-link" type="button" onClick={() => openCase(item)}><strong>{item.caseNumber}</strong><span>{item.title}</span></button></td>
            <td>{item.processUnitName}</td><td>{formatLabel(item.type)}</td><td><span className={`status status-${item.status.toLowerCase()}`}>{formatLabel(item.status)}</span></td><td><span className={`severity severity-${item.severity.toLowerCase()}`}>{item.severity}</span></td><td>{formatDate(item.reportedAt)}</td><td><ChevronRight size={18} /></td>
          </tr>)}</tbody>
        </table></div>
      </section>
    </main>

    {selectedCase && <div className="drawer-backdrop" onMouseDown={() => setSelectedCase(null)}><aside className="case-drawer" aria-label={`Case ${selectedCase.summary.caseNumber}`} onMouseDown={(event) => event.stopPropagation()}>
      <div className="drawer-header"><div><span>{selectedCase.summary.caseNumber}</span><h2>{selectedCase.summary.title}</h2></div><button className="icon-button" type="button" onClick={() => setSelectedCase(null)} title="Close case details"><X size={20} /></button></div>
      <div className="drawer-body">
        <div className="detail-badges"><span className={`severity severity-${selectedCase.summary.severity.toLowerCase()}`}>{selectedCase.summary.severity}</span><span className={`status status-${selectedCase.summary.status.toLowerCase()}`}>{formatLabel(selectedCase.summary.status)}</span></div>
        <dl><div><dt>Process unit</dt><dd>{selectedCase.summary.processUnitName}</dd></div><div><dt>Case type</dt><dd>{formatLabel(selectedCase.summary.type)}</dd></div><div><dt>Reported</dt><dd>{formatDate(selectedCase.summary.reportedAt)}</dd></div><div><dt>Regulatory notice</dt><dd>{selectedCase.regulatoryNotifiable ? 'Required' : 'Not required'}</dd></div></dl>
        <section><h3>Summary</h3><p>{selectedCase.description}</p></section>
        <section><h3>Timeline</h3><ol className="timeline">{selectedCase.timeline.map((entry) => <li key={entry.id}><span></span><div><strong>{entry.entryType}</strong><time>{formatDate(entry.occurredAt)} · {entry.authorName}</time><p>{entry.content}</p></div></li>)}</ol></section>
      </div>
    </aside></div>}
    {buddyOpen && <CaseBuddy onClose={() => setBuddyOpen(false)} />}
    {foundryOpen && <FoundryBuddy onClose={() => setFoundryOpen(false)} />}
  </div>
}

export default App