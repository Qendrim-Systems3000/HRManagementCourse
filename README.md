# HR Management Course API

A multi-tenant ASP.NET Core Web API for managing courses, course types, employees, and enrollments. Supports JWT authentication with refresh tokens, role-based access (Admin, HRUser), and tenant isolation by district (LeaProfileId).

## Tech Stack

- **.NET 10** – Web API
- **ASP.NET Core Identity** – Users and roles
- **Entity Framework Core 10** – SQL Server, migrations
- **JWT Bearer** – Access tokens + refresh tokens (rotation)
- **AutoMapper** – Entity ↔ DTO mapping
- **Swagger / OpenAPI** – API documentation

## Solution Structure

| Project | Description |
|--------|-------------|
| **HRManagement.Api** | Controllers, middleware, JWT/Swagger config |
| **HRManagement.Application** | DTOs, interfaces, application services, AutoMapper profiles |
| **HRManagement.Domain** | Entities (Course, CourseType, Employee, EmployeeCourse), base types |
| **HRManagement.Infrastructure** | EF DbContext, Identity, repositories, AuthService, seeding |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or SQL Express), or update the connection string for your server

## Getting Started

### 1. Clone and restore

```bash
git clone <repo-url>
cd HRManagementCourse
dotnet restore
```

### 2. Database

Set the connection string in `HRManagement.Api/appsettings.json` if needed (default: `Server=.;Database=HRManagementDb;Trusted_Connection=True;...`).

Create/update the database:

```bash
dotnet ef database update --project HRManagement.Infrastructure --startup-project HRManagement.Api
```

To recreate from scratch:

```bash
dotnet ef database drop --project HRManagement.Infrastructure --startup-project HRManagement.Api --force
dotnet ef database update --project HRManagement.Infrastructure --startup-project HRManagement.Api
```

### 3. Run the API

```bash
dotnet run --project HRManagement.Api
```

By default the API listens on **http://localhost:5150**. Swagger UI: **http://localhost:5150/swagger**.

On first run, the app seeds:

- Roles: **Admin**, **HRUser**
- Sample course types, courses, employees, and enrollments for tenant/department **1**

## Configuration

Key settings in `appsettings.json`:

| Section | Key | Description |
|--------|-----|-------------|
| **ConnectionStrings** | DefaultConnection | SQL Server connection string |
| **JWT** | Secret | Signing key (min 32 characters) |
| **JWT** | ValidIssuer / ValidAudience | Token issuer/audience |
| **JWT** | AccessTokenExpirationMinutes | Access token lifetime (default: 15) |
| **JWT** | RefreshTokenExpirationDays | Refresh token lifetime (default: 7) |

## API Overview

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Auth/register` | Register user (email, password, LeaProfileId, role) |
| POST | `/api/Auth/login` | Login → returns `token`, `refreshToken`, `refreshTokenExpiresAt` |
| POST | `/api/Auth/refresh` | Body: `{ "refreshToken": "..." }` → new access + refresh token |
| POST | `/api/Auth/revoke` | Body: `{ "refreshToken": "..." }` → revoke refresh token (e.g. logout) |

Use the **access token** in the `Authorization: Bearer <token>` header for protected endpoints.

### Course Types

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/CourseTypes` | Yes | List course types for current tenant |
| GET | `/api/CourseTypes/{id}` | Yes | Get by id |
| POST | `/api/CourseTypes` | Admin, HRUser | Create |
| PUT | `/api/CourseTypes/{id}` | Admin, HRUser | Update |
| DELETE | `/api/CourseTypes/{id}` | Admin | Delete |

### Courses

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Courses` | Yes | List (optional query: typeId, date, approved) |
| GET | `/api/Courses/{id}` | Yes | Get by id |
| POST | `/api/Courses` | Admin, HRUser | Create (requires valid `courseTypeId`) |
| PUT | `/api/Courses/{id}` | Admin, HRUser | Update |
| DELETE | `/api/Courses/{id}` | Admin | Delete |

### Employee Courses (Enrollments)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/EmployeeCourses/enroll` | Admin, HRUser | Enroll employee in course |
| GET | `/api/EmployeeCourses` | Yes | List (optional: employeeId, courseId) |
| GET | `/api/EmployeeCourses/employee/{employeeId}` | Yes | Transcript for employee |
| PUT | `/api/EmployeeCourses/{id}` | Admin, HRUser | Update enrollment |
| DELETE | `/api/EmployeeCourses/{id}` | Admin | Remove enrollment |

## Multi-Tenancy

Data is isolated by **LeaProfileId** (tenant/department). The API reads the tenant from the user’s JWT claim `LeaProfileId`. All queries and inserts for courses, course types, employees, and enrollments are scoped to that tenant. Register and login set the user’s `LeaProfileId` and role (Admin or HRUser).

## Error Handling

- **Global exception middleware** returns RFC 7807–style problem details (JSON) with `type`, `title`, `status`, `detail`, `traceId`, `exceptionType`.
- **404** for missing resources (e.g. “Course type does not exist”).
- **400** for validation/business rule errors (e.g. duplicate course, invalid operation).
- **401** for missing or invalid token; **403** when the user’s role is not allowed.
- Stack traces are not included in responses; full exceptions are logged server-side.

## License

Use as needed for your course or organization.
