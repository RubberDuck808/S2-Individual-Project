# Core Workflows

## Application lifecycle

1. **Student applies** — `ApplicationService.CreateAsync` rejects duplicate applications (`ExistsAsync` check throws a `ValidationException`) and creates an `Application` with `StatusId = 1` (Pending).
2. **Landlord reviews** — `ApplicationService.GetByAccommodationIdAsync` lists every application for one of the landlord's listings.
3. **Landlord selects a tenant** — `ApplicationService.SelectApplicantAsync(applicationId, accommodationId)`:
   - Marks the chosen application as selected (`MarkAsSelectedAsync`).
   - Rejects every other pending application for that same accommodation in one call (`RejectOthersAsync`) — so accepting one applicant automatically declines the rest.
4. **Booking is created from the selected application** — `BookingService.CreateAsync(studentId, accommodationId, applicationId)` creates a `Booking` row (`StatusId = 1`), currently with a hardcoded 6-month term (`StartDate = now`, `EndDate = now + 6 months`) and `TotalAmount = 0` — both are placeholders, not yet wired to the listing's actual rent or a landlord-chosen date range.

A landlord can also update an application's status directly via `ApplicationService.UpdateStatusAsync` outside the select/reject flow.

## Booking lifecycle

- **Update** — `BookingService.UpdateAsync` patches `StartDate`/`EndDate`/`StatusId` if provided.
- **Cancel** — `BookingService.CancelAsync` hardcodes `StatusId = 3` (Cancelled).
- **Named status update with ownership check** — `BookingService.UpdateStatusAsync(bookingId, statusName, studentId?)`:
  - Looks up the target status by name via `IStatusRepository.GetIdByNameAsync` (so callers pass `"Cancelled"` rather than a magic number).
  - If a `studentId` is supplied, verifies the booking actually belongs to that student and throws `ForbiddenException` (`UnauthorizedBookingModification`) otherwise — this is the access-control check protecting students from modifying each other's bookings.

Status IDs (`1`, `2`, `3`, ...) map to rows in the shared `Status` table; the inline `// e.g. Pending` / `// Cancelled` comments in the service code are the only place these magic numbers are currently documented, so treat `Status` table contents as load-bearing when changing this code.

## Listing geocoding

When a landlord creates or edits an `Accommodation`, `GeoLocationService` calls `IGoogleMapsApiWrapper` to geocode the listing's address into `Latitude`/`Longitude`, and to derive `City`/`PostCode`. The geocoded coordinates are then compared against the seeded `University` rows to auto-link the listing to its nearest university (`UniversityId`).

## Error handling

Custom exceptions in `BLL/Exceptions/` (`NotFoundException`, `ValidationException`, `ForbiddenException`, all deriving from a common base `ApplicationException`) are thrown by services for expected failure cases (missing entity, invalid input, unauthorized action) and are translated into the appropriate page response by the Razor Pages layer / `UseStatusCodePages` middleware in `Program.cs` (404 → `/NotFound`, 403 → `/AccessDenied`).
