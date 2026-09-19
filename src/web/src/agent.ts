const requireEnvironmentVariable = (name: string, value: string | undefined) => {
  if (!value) {
    throw new Error(`${name} is required. Copy .env.example to .env and configure it.`)
  }

  return value
}

const brokerBaseUrl = requireEnvironmentVariable('VITE_BROKER_BASE_URL', import.meta.env.VITE_BROKER_BASE_URL).replace(/\/$/, '')
export const brokerScope = requireEnvironmentVariable('VITE_BROKER_SCOPE', import.meta.env.VITE_BROKER_SCOPE)

export async function askCaseBuddy(message: string, accessToken: string, signal?: AbortSignal): Promise<string> {
  const response = await fetch(`${brokerBaseUrl}/api/chat`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${accessToken}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ message }),
    signal,
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null) as { detail?: string } | null
    throw new Error(problem?.detail ?? `Case Buddy request failed (${response.status}).`)
  }

  const result = await response.json() as { answer: string }
  return result.answer
}