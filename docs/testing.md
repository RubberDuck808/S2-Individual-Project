# Testing

`Tests/` — xUnit + Moq, one file per BLL service, exercising the service layer in isolation by mocking its repository/wrapper dependencies (no real database or HTTP calls).

| File | Covers |
|---|---|
| `AccommodationServiceTests.cs` | Listing CRUD, filtering, availability logic (largest suite) |
| `AccountServiceTests.cs` | Registration / login |
| `ApplicationServiceTests.cs` | Application create/duplicate-check, status updates, select/reject-others flow |
| `BookingServiceTests.cs` | Booking creation, cancellation, status updates with ownership check |
| `GeoLocationServiceTests.cs` | Address → coordinates geocoding via the mocked `IGoogleMapsApiWrapper` |
| `LandlordServiceTests.cs` | Landlord profile management & verification |
| `StudentServiceTests.cs` | Student profile management |

## Running tests

```sh
cd Tests
dotnet test
```

Or from the solution root:

```sh
dotnet test UniNest/UniNest.sln
```

No integration tests exist against a real SQL Server instance — every test mocks the relevant `DAL` interface (`IAccommodationRepository`, `IBookingRepository`, etc.) with Moq, so test runs don't require `UNINEST_CONNECTION_STRING` or `GOOGLE_API_KEY` to be set.
