const requireEnvironmentVariable = (name: string, value: string | undefined) => {
  if (!value) {
    throw new Error(`${name} is required. Copy .env.example to .env and configure it.`)
  }

  return value
}

export const copilotConfiguration = {
  environmentId: requireEnvironmentVariable(
    'VITE_COPILOT_STUDIO_ENVIRONMENT_ID',
    import.meta.env.VITE_COPILOT_STUDIO_ENVIRONMENT_ID,
  ),
  agentIdentifier: requireEnvironmentVariable(
    'VITE_COPILOT_STUDIO_SCHEMA_NAME',
    import.meta.env.VITE_COPILOT_STUDIO_SCHEMA_NAME,
  ),
}

export const copilotScope = 'https://api.powerplatform.com/.default'