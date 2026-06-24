# Environment Variables

UniNest reads two required settings from **environment variables only** (not from `appsettings.json`) — `UI/Program.cs` throws on startup if either is missing:

| Variable | Used by | Description |
|---|---|---|
| `UNINEST_CONNECTION_STRING` | All `DAL/Repositories/*` classes | SQL Server connection string. Passed directly into each repository's constructor (no EF Core, no `appsettings.json` fallback — see [database.md](database.md)). |
| `GOOGLE_API_KEY` | `GeoLocationService` via `GoogleMapsApiWrapper` | Google Maps Geocoding API key, used to turn a listing's address into coordinates and link it to the nearest university. |

## Setting them locally

PowerShell (current session only):

```powershell
$env:UNINEST_CONNECTION_STRING = "Server=localhost;Database=UniNest;Trusted_Connection=True;TrustServerCertificate=True;"
$env:GOOGLE_API_KEY = "<your-google-maps-api-key>"
```

Or persist them for the user account:

```powershell
[System.Environment]::SetEnvironmentVariable("UNINEST_CONNECTION_STRING", "<connection-string>", "User")
[System.Environment]::SetEnvironmentVariable("GOOGLE_API_KEY", "<api-key>", "User")
```

In Visual Studio, you can also set them under `UI` project → Properties → Debug → Environment variables, so they're picked up when running via F5.

## Other configuration (`UI/appsettings.json`)

These are read normally via `IConfiguration`, not environment variables:

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | Present but unused — `UNINEST_CONNECTION_STRING` is what's actually wired into the repositories. Left blank in the committed file. |
| `MailboxAddress:Name` / `MailboxAddress:Address` | Sender identity used by `EmailService` (MailKit) for outgoing email. |
| `GoogleMaps:ApiKey` | Present but unused — the wrapper reads `GOOGLE_API_KEY` from the environment instead. |
| `Logging:LogLevel` | Standard ASP.NET Core logging configuration. |
