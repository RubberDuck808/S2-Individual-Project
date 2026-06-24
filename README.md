## GitHub
https://github.com/RubberDuck808/S2-Individual-Project

# UniNest — Student Accommodation Platform

UniNest connects students to verified landlords and listings near their university. Students browse accommodations, apply, and track bookings; landlords list and manage properties and review applicants; admins verify landlords and manage university data.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend / Frontend | ASP.NET Core 9 (Razor Pages), .NET 8 target |
| Data access | Hand-written SQL via `Microsoft.Data.SqlClient` — no EF Core, no migrations |
| Database | SQL Server |
| Authentication | Cookie-based, custom `UniNestAuth` scheme, role-based (Student/Landlord/Admin) |
| Mapping | AutoMapper |
| Email | MailKit |
| External API | Google Maps Geocoding API |
| Testing | xUnit + Moq |

## Project Structure

```
S2-Individual-Project/
├── Domain/        # Entity classes (Accommodation, Landlord, Student, Application, Booking, ...)
├── DAL/           # Repositories + interfaces — raw SQL via Microsoft.Data.SqlClient
├── BLL/           # Services, DTOs, AutoMapper profiles, custom exceptions
├── APIWrapper/    # Google Maps Geocoding API wrapper
├── UI/            # Razor Pages app: Pages/, ViewModels/, wwwroot/, Program.cs
├── Tests/         # xUnit + Moq unit tests (one file per BLL service)
├── UniNest/       # Solution file (UniNest.sln)
└── docs/          # Full documentation (see below)
```

## Documentation

Detailed docs live in [`docs/`](docs/README.md):

- [Architecture](docs/architecture.md) — layering, request flow, auth, Razor Pages layout
- [Database](docs/database.md) — tables, relationships, and the raw-SQL data access approach
- [Workflows](docs/workflows.md) — application/booking lifecycle, status transitions, geocoding
- [Testing](docs/testing.md) — unit test suite and how to run it
- [Environment variables](docs/environment.md) — required env vars and config keys

## Setup & Installation

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (local or remote), with the application schema already created (see [docs/database.md](docs/database.md) — there are no migrations in this repo)
- A Google Maps API key with the Geocoding API enabled

### 1. Configure environment variables

The app **will not start** without these two (see [docs/environment.md](docs/environment.md) for details):

```powershell
$env:UNINEST_CONNECTION_STRING = "Server=localhost;Database=UniNest;Trusted_Connection=True;TrustServerCertificate=True;"
$env:GOOGLE_API_KEY = "<your-google-maps-api-key>"
```

### 2. Run

```sh
dotnet run --project UI
```

Or open `UniNest/UniNest.sln` in Visual Studio and run the `UI` project.

## Running Tests

```sh
dotnet test UniNest/UniNest.sln
```

Tests mock all repository/wrapper dependencies, so no database connection or API key is required to run them — see [docs/testing.md](docs/testing.md).

## Core Features

- User authentication with roles (Student, Landlord, Admin)
- CRUD for accommodations, landlords, students
- Application system with status tracking and a select-one/auto-reject-rest flow
- Booking system tied to approved applications
- Listing geocoding and nearest-university linking via Google Maps
- Admin tools for landlord/student/university management

## License

[MIT](LICENSE)
