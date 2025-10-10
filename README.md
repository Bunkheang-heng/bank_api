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
- OpenAPI (Development only):
  - HTTP: `http://localhost:5175/openapi/v1.json`
  - HTTPS: `https://localhost:7256/openapi/v1.json`

## Swagger UI (optional)
If you want a UI for testing the API:
1. Add package:
```bash
dotnet add package Swashbuckle.AspNetCore
```
2. In `Program.cs`:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// after app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "v1"));
```
3. Run the app and open `https://localhost:7256` (or `http://localhost:5175`).

## Configuration
- `appsettings.json` and `appsettings.Development.json` for environment-specific settings
- Do not commit secrets; use user secrets, environment variables, or keep secrets only in `appsettings.Development.json` locally.

## .http file (optional testing)
Use `bank_api.http` with VS Code REST Client extension to quickly send requests during development.

## Git
A `.gitignore` is included to exclude build outputs (`bin/`, `obj/`), IDE files, logs, coverage, and local environment files.

## Troubleshooting
- Warning: "Failed to determine the https port for redirect." → You’re running HTTP-only while `app.UseHttpsRedirection()` is enabled. Either run with the `https` profile or remove that middleware.
- HTTPS connection refused → Ensure the dev certificate is trusted and run with `--launch-profile https`.
