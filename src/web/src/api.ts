import type { CaseDetail, CaseSeverity, CaseStatus, CaseSummary, PagedResult } from './types'

interface CaseFilters {
  search: string
  status: '' | CaseStatus
  severity: '' | CaseSeverity
}

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')

async function getJson<T>(path: string, accessToken: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    signal,
    headers: { Authorization: `Bearer ${accessToken}` },
  })
  if (!response.ok) throw new Error(`Request failed with status ${response.status}`)
  return response.json() as Promise<T>
}

export function listCases(filters: CaseFilters, accessToken: string, signal?: AbortSignal) {
  const query = new URLSearchParams()
  if (filters.search.trim()) query.set('search', filters.search.trim())
  if (filters.status) query.set('status', filters.status)
  if (filters.severity) query.set('severity', filters.severity)
  return getJson<PagedResult<CaseSummary>>(`/api/v1/cases?${query}`, accessToken, signal)
}

export function getCase(id: string, accessToken: string, signal?: AbortSignal) {
  return getJson<CaseDetail>(`/api/v1/cases/${id}`, accessToken, signal)
}