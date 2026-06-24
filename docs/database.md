# Database

UniNest stores data in **SQL Server**, accessed entirely through hand-written SQL in `DAL/Repositories/` via `Microsoft.Data.SqlClient` — there is **no EF Core `DbContext`, no migrations folder, and no ORM** mapping in this repo, despite `Microsoft.AspNetCore.Identity.EntityFrameworkCore` being referenced in `UI.csproj` (it's an unused dependency; Identity tables are read/written manually instead). The schema itself is not version-controlled in this repo — it must exist already in the target SQL Server instance pointed to by `UNINEST_CONNECTION_STRING`, including the standard ASP.NET Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.) plus the application tables below.

## Application tables (from `Domain/Models/`)

| Table (entity) | Key columns | Notes |
|---|---|---|
| `University` | `UniversityId`, `Name`, `City`, `Location`, `Latitude`, `Longitude` | Seeded reference data; listings link to the nearest one via geocoding. |
| `Landlord` | `LandlordId`, `UserId` (FK → `AspNetUsers.Id`), `FirstName`/`LastName`/`Email`/`PhoneNumber`, `CompanyName`, `TaxIdentificationNumber`, `IsVerified`, `VerificationDate`, `CreatedAt`, `UpdatedAt` | One-to-one with an Identity user; one-to-many with `Accommodation`. |
| `Student` | `StudentId`, `UserId` (FK → `AspNetUsers.Id`), `UniversityId` (FK), `DateOfBirth`, `PhoneNumber`, `EmergencyContact`, `EmergencyPhone`, `ProfileImageUrl`, `IsVerified`, `CreatedAt`, `UpdatedAt` | One-to-one with an Identity user. |
| `AccommodationType` | `AccommodationTypeId`, `Name` | Lookup table (e.g. studio, shared room). |
| `Amenity` | `AmenityId`, `Name`, `IconName` | Lookup table. |
| `Accommodation` | `AccommodationId`, `Title`, `Description`, `Address`/`PostCode`/`City`/`Country`, `Latitude`/`Longitude`, `MonthlyRent`, `Size`, `MaxOccupants`, `IsAvailable`, `AvailableFrom`, `LandlordId` (FK), `AccommodationTypeId` (FK), `UniversityId` (FK) | Core listing entity. Coordinates populated via Google Maps geocoding when created/edited. |
| `AccommodationAmenity` | `AccommodationId`, `AmenityId` | Many-to-many join table between `Accommodation` and `Amenity`. |
| `AccommodationImage` | `ImageId`, `AccommodationId` (FK), `ImageUrl`, `Description`, `UploadedAt` | One-to-many photos per listing. |
| `Status` | `StatusId`, `Name` | Shared lookup table for both `Application` and `Booking` states (e.g. Pending/Approved/Rejected, Active/Cancelled). |
| `Application` | `ApplicationId`, `StudentId` (FK), `AccommodationId` (FK), `StatusId` (FK), `ApplicationDate` | A student's request to rent a listing. |
| `Booking` | `BookingId`, `StartDate`, `EndDate`, `TotalAmount`, `StudentId` (FK), `AccommodationId` (FK), `StatusId` (FK), `ApplicationId` (FK) | Created once an application is approved; tracks the confirmed stay. |

## Relationships at a glance

- A `Landlord` owns many `Accommodation`s.
- An `Accommodation` belongs to one `University` and one `AccommodationType`, and has many `Amenity`s (via `AccommodationAmenity`) and many `AccommodationImage`s.
- A `Student` submits many `Application`s, each against one `Accommodation`.
- An approved `Application` produces one `Booking`.
- `Status` is reused by both `Application` and `Booking` rather than having separate enums per entity.

## Identity integration

`Landlord.UserId` and `Student.UserId` are foreign keys into `AspNetUsers.Id` (a GUID string), linking each role-specific profile to its login credentials. Registration (`AccountService` → `UserRepository.CreateUserAsync`) inserts directly into `AspNetUsers` with the standard Identity column set, and `AssignRoleAsync` inserts into the Identity role-join table — entirely by hand, since the app doesn't use `UserManager`/`SignInManager`.

## Working with the schema

Because there are no migrations, schema changes must be applied manually to your SQL Server instance (e.g. via a script or SSMS) and kept in sync with the repository classes in `DAL/Repositories/` by hand. If you add a column or table, update the corresponding repository's SQL and the `Domain` entity together.
