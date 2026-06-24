# Architecture

UniNest is an ASP.NET Core Razor Pages app following a strict four-layer architecture: **Domain → DAL → BLL → UI**, each its own .csproj, referencing only the layer directly below it.

```
UI  ──▶  BLL  ──▶  DAL  ──▶  SQL Server
 │        │         │
 │        │         └─ Domain (entities, shared by every layer)
 │        └─ APIWrapper (external HTTP APIs, e.g. Google Maps)
 └─ Razor Pages, ViewModels, wwwroot
```

## Projects

| Project | Role |
|---|---|
| `Domain` | Plain entity classes (`Accommodation`, `Landlord`, `Student`, `Application`, `Booking`, `University`, `Amenity`, `AccommodationType`, `AccommodationImage`, `Status`, `ApplicationUser`). No dependencies on other projects. |
| `DAL` | Data Access Layer. One repository class + interface per entity, talking to SQL Server directly via `Microsoft.Data.SqlClient` (hand-written SQL, no ORM — see [database.md](database.md)). |
| `BLL` | Business Logic Layer. Services (one per domain area) implement validation, workflow rules (e.g. booking date overlap checks, application status transitions), and DTO mapping via AutoMapper. Custom exceptions live in `BLL/Exceptions`. |
| `APIWrapper` | Thin wrapper around the Google Maps Geocoding API (`GoogleMapsApiWrapper`), used by `GeoLocationService` to turn a listing's address into coordinates and the nearest university. |
| `UI` | ASP.NET Core Razor Pages app: pages, page models, ViewModels, static assets, `Program.cs` (DI + middleware), `appsettings.json`. |
| `Tests` | xUnit + Moq unit tests for the BLL services, mocking repository interfaces. |

## Request flow

```
Razor Page (PageModel) → BLL Service (interface) → DAL Repository (interface) → SQL Server
                              │
                              └─ AutoMapper: Domain entity ⇄ DTO
```

- Page models depend on **service interfaces** (`IAccommodationService`, `IBookingService`, etc.), injected via constructor DI, registered in `UI/Program.cs`.
- Services depend on **repository interfaces**, also DI-registered, each constructed with the SQL Server connection string read from the `UNINEST_CONNECTION_STRING` environment variable.
- DTOs (`BLL/DTOs/<Area>/`) are the only objects that cross from BLL into UI — page models never receive raw `Domain` entities directly from a service that exposes data to a view.

## Authentication & authorization

- Cookie-based authentication under a custom scheme, `UniNestAuth` (`UI/Program.cs`), backed by the standard ASP.NET Identity `AspNetUsers`/`AspNetRoles` tables — but accessed through hand-written SQL in `UserRepository`, **not** through Identity's `UserManager`/`SignInManager` or EF Core. `AccountService` handles registration/login and hashes passwords with `IPasswordHasher<object>`.
- Three roles: `Student`, `Landlord`, `Admin`. Pages are protected with `[Authorize]` and role checks in the page model; unauthorized/unauthenticated requests are redirected to `/Login` or `/AccessDenied`.
- Session cookie expires after 15 minutes with sliding expiration.

## External integration

`GeoLocationService` (BLL) → `IGoogleMapsApiWrapper` (APIWrapper) → Google Maps Geocoding API. Used when a landlord creates/edits a listing: the address is geocoded to lat/lng, and the listing is auto-linked to its nearest `University` row. Requires `GOOGLE_API_KEY` to be set; the app throws on startup if it's missing.

## UI structure (`UI/Pages/`)

```
Pages/
├── Index.cshtml                  # Homepage
├── Login.cshtml
├── Register/                     # Student & landlord registration
├── Listings/                     # Public browse (Index) + Detail
├── Info/                         # AboutUs, Contact, Privacy
├── Error/                        # NotFound (404), AccessDenied (403), Error
├── Dashboard/
│   ├── Index.cshtml              # Role-based dashboard landing
│   ├── Logout.cshtml
│   ├── Student/                  # Dashboard, MyApplications, MyBookings
│   ├── Landlord/                 # Dashboard, MyListings, CreateListing, EditListing, EditAccount
│   └── Admin/                    # ManageLandlords, ManageStudents, ManageUniversities
└── Shared/                       # _Layout, _StudentLayout, _LandlordLayout, _ValidationScriptsPartial
```
