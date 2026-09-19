# Case Reporting Web

React 19 and Vite frontend for the refinery case app. It provides the case console, MSAL sign-in, and the embedded Refining Case Buddy panel.

## Run locally

```powershell
npm install
npm run dev
```

Vite uses `src/web/.env.example` as the catalog for browser configuration. Local development proxies API requests to `http://localhost:5016`.

## Validate

```powershell
npm run lint
npm run build
```

Production builds require the `VITE_ENTRA_*`, `VITE_API_*`, and `VITE_COPILOT_STUDIO_*` values listed in `.env.example`. The repository deployment script supplies them during its build.

For architecture and local API setup, see the [repository README](../../README.md). For connector and agent configuration, see [Power Platform and Refining Case Buddy](../../docs/power-platform.md).