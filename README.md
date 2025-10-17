# Cyber Bank - bank_api

A minimal ASP.NET Core API skeleton exposing a sample `GET /weatherforecast` endpoint and OpenAPI document.

## Requirements
- .NET SDK 9.0+
- macOS dev HTTPS cert trusted (see below)

## Getting Started

### Clone and enter project
```bash
cd /Users/bunkheangheng/Desktop/Project/cyber_bank/bank_api
```

### Trust HTTPS Dev Certificate (first time only)
```bash
dotnet dev-certs https --trust
```

### Run (HTTP)
```bash
dotnet run
```
You should see logs similar to:
- Now listening on: `http://localhost:5175`

Open the sample endpoint:
- Browser: `http://localhost:5175/weatherforecast`
- Curl:
```bash
curl http://localhost:5175/weatherforecast
```

### Run with HTTPS
```bash
dotnet run --launch-profile https
```
You should see logs similar to:
- Now listening on: `https://localhost:7256`

Test:
```bash
curl -k https://localhost:7256/weatherforecast
```

### Hot Reload during development
```bash
dotnet watch
```

## Endpoints
- `GET /weatherforecast` → returns an array of 5 forecast items
- `POST /auth/register` → create user
- `POST /auth/login` → get JWT
- `GET /me` → get current user (requires Bearer token)
- OpenAPI (Development only):
  - HTTP: `http://localhost:5175/openapi/v1.json`
  - HTTPS: `https://localhost:7256/openapi/v1.json`

## Swagger UI
- Available in Development at the same port as the API.
- UI: `http://localhost:5175/swagger` (or `https://localhost:7256/swagger`)
- OpenAPI JSON is still at `/openapi/v1.json`.

## Auth usage

### Register
```bash
curl -X POST http://localhost:5175/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"username":"alice","password":"P@ssw0rd!"}'
```
Returns 201 Created.

### Login
```bash
curl -X POST http://localhost:5175/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"username":"alice","password":"P@ssw0rd!"}'
```
Response example:
```json
{
  "accessToken": "<JWT>",
  "tokenType": "Bearer",
  "expiresAtUtc": "2025-10-16T12:34:56Z"
}
```

### Authenticated request
```bash
TOKEN="$(curl -s -X POST http://localhost:5175/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"username":"alice","password":"P@ssw0rd!"}' | jq -r .accessToken)"

curl http://localhost:5175/me -H "Authorization: Bearer $TOKEN"
```

## Configuration
- `appsettings.json` and `appsettings.Development.json` for environment-specific settings
- JWT config under `Jwt` section: `Issuer`, `Audience`, `Key`, `ExpiresMinutes`
- `Jwt:Key` must be at least 32 bytes (256 bits) when UTF-8 encoded for HS256.
- Do not commit secrets; keep `Jwt:Key` secure. For dev, it’s fine to keep it in `appsettings.Development.json` locally. For prod, use environment variables or a secret store.

## .http file (optional testing)
Use `bank_api.http` with VS Code REST Client extension to quickly send requests during development.

## Git
A `.gitignore` is included to exclude build outputs (`bin/`, `obj/`), IDE files, logs, coverage, and local environment files. Also ignores `Data/users.json` created by the local user store.

## Troubleshooting
- Warning: "Failed to determine the https port for redirect." → You’re running HTTP-only while `app.UseHttpsRedirection()` is enabled. Either run with the `https` profile or remove that middleware.
- HTTPS connection refused → Ensure the dev certificate is trusted and run with `--launch-profile https`.
