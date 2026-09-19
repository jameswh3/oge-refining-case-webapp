export type CaseStatus = 'Open' | 'InProgress' | 'OnHold' | 'Closed'
export type CaseSeverity = 'Informational' | 'Minor' | 'Moderate' | 'Major' | 'Critical'
export type CaseType =
  | 'Incident'
  | 'NearMiss'
  | 'NonConformance'
  | 'SafetyObservation'
  | 'MaintenanceIssue'
  | 'EnvironmentalConcern'

export interface CaseSummary {
  id: string
  caseNumber: string
  title: string
  refineryName: string
  processUnitName: string
  type: CaseType
  status: CaseStatus
  severity: CaseSeverity
  reportedAt: string
  updatedAt: string
}

export interface CaseTimelineEntry {
  id: string
  entryType: string
  authorName: string
  occurredAt: string
  content: string
}

export interface CaseDetail {
  summary: CaseSummary
  description: string
  regulatoryNotifiable: boolean
  timeline: CaseTimelineEntry[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}