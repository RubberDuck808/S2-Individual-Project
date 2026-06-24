## GitHub
https://github.com/RubberDuck808/S2-Individual-Project

# UniNest — Student Accommodation Platform

**UniNest** is a student accommodation web platform built with ASP.NET Core Razor Pages, using a layered architecture (DAL, BLL, UI) and backed by a SQL Server database. The goal is to make it easier for students to find verified housing near universities — with features for landlords, booking, and application tracking.

---

## Table of Contents

- [About the Project](#about-the-project)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Core Features](#core-features)
- [Database Design](#database-design)
- [Setup & Installation](#setup--installation)
- [Git Workflow](#git-workflow)
- [Contributing](#contributing)

---

## 📖 About the Project

UniNest is built as a semester 2 individual project at Fontys ICT. The platform connects students to verified landlords and lets them:

- Browse accommodation listings
- Submit applications
- Manage bookings
- Verify landlord credentials

The system uses EF Core for data access, Identity for authentication, and AutoMapper for DTO conversion.

---

##  Technologies Used

| Area              | Technology                         |
|-------------------|-------------------------------------|
| Language          | C# (.NET 8)                        |
| Frontend          | Razor Pages (ASP.NET Core)         |
| ORM               | Entity Framework Core              |
| Database          | SQL Server                         |
| Authentication    | ASP.NET Identity                   |
| Mapping           | AutoMapper                         |
| Logging           | ILogger<T>                         |
| Version Control   | Git + GitHub + GitLab              |

---

## Project Structure

UniNest/
│
├── BLL/ # Business Logic Layer
│ ├── DTOs/ # Data Transfer Objects
│ ├── Interfaces/ # Business Interfaces
│ ├── Services/ # Service Implementations
│ └── MappingProfiles/ # AutoMapper config
│
├── DAL/ # Data Access Layer
│ ├── Data/ # DbContext & SeedData
│ ├── Entities/ # EF Core Models
│ ├── Interfaces/ # Repository Interfaces
│ └── Repositories/ # Repository Implementations
│
├── Pages/ # Razor Pages (UI Layer)
│ ├── Landlord.cshtml # Example UI pages
│ └── Listings.cshtml
│
├── appsettings.json # App config
└── Program.cs # Entry point / service config



---

## Core Features

- 🔒 User authentication with roles (Landlord, Student)
- 🏠 CRUD for Accommodations, Landlords, Students
- 📄 Application system (with status tracking)
- 📆 Booking system with availability control
- 🧩 EF Core navigation + relationship mapping
- 📦 DTO pattern for clean data handling
- ✅ Admin seeding logic (universities, amenities)

---

## 🗄️ Database Design (Simplified)

**Main Entities:**

- `Student`
- `Landlord`
- `Accommodation`
- `University`
- `Amenity`
- `Application`
- `Booking`
- `Status` (shared across booking & application)

**Key Relationships:**

- A `Student` can apply to or book an `Accommodation`
- A `Landlord` owns multiple `Accommodations`
- Each `Accommodation` belongs to one `University`
- Many-to-many between `Accommodation` and `Amenity`

> Schema is enforced via Fluent API in `AppDbContext.cs`.
