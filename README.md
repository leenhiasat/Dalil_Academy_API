# Dalil Academy – LMS API

## Project Overview

Dalil Academy is a Learning Management System (LMS) built using ASP.NET Core Web API, Entity Framework Core, and PostgreSQL.

The project follows Onion Architecture and includes user authentication, course management, student enrollment, and PayPal payment integration.

### Technologies Used

- C# / ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication & Authorization
- BCrypt Password Hashing
- PayPal REST API (Sandbox)
- Swagger / OpenAPI

### Project Structure

The solution consists of three main projects:

- **Dalil_Academy.Core:** Contains entities, enums, DTOs, interfaces, and business contracts.
- **Dalil_Academy.Infra:** Contains repositories, database configurations, migrations, payment gateway, and services.
- **Dalil_Academy.API:** Contains controllers, middleware, dependency injection, and application configuration.

---

## 1. Prerequisites

Before running the project, make sure you have:

- .NET SDK compatible with the project's TargetFramework.
- PostgreSQL.
- Visual Studio or Visual Studio Code.
- PayPal Developer Sandbox Account.

---

## 2. Database Configuration

The project uses PostgreSQL as its database provider.

1. Make sure PostgreSQL is running.
2. Open `appsettings.json` in the API project.
3. Configure the database connection string using your local PostgreSQL credentials.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=DalilAcademyDB2;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  }
}
```

**Note:** Use the connection string key already configured in the project if it differs from `DefaultConnection`.

---

## 3. PayPal Sandbox Configuration

The project integrates with PayPal Sandbox for testing payments without using real money.

### Create Sandbox Accounts

1. Visit https://developer.paypal.com/
2. Sign in to your PayPal Developer account.
3. Create a Sandbox application.
4. Obtain the Sandbox Client ID and Client Secret.
5. Create or use:
   - Personal Account (Buyer).
   - Business Account (Seller).

### Configure PayPal Settings

Add the following configuration to `appsettings.json`:

```json
{
  "PayPal": {
    "BaseUrl": "https://api-m.sandbox.paypal.com",
    "ReturnUrl": "YOUR_SWAGGER_RETURN_URL",
    "CancelUrl": "YOUR_SWAGGER_CANCEL_URL",
    "JodToUsdRate": 1.41
  }
}
```

Replace the Return URL and Cancel URL with the appropriate local Swagger URLs used by your application.

### Configure User Secrets

PayPal credentials must be stored securely using ASP.NET Core User Secrets.

From the solution directory, initialize User Secrets for the API project:

```powershell
dotnet user-secrets init --project Dalil_Academy.API
```

Add the Sandbox credentials:

```powershell
dotnet user-secrets set "PayPal:ClientId" "YOUR_CLIENT_ID" --project Dalil_Academy.API

dotnet user-secrets set "PayPal:ClientSecret" "YOUR_CLIENT_SECRET" --project Dalil_Academy.API
```

**Important:**
- Never store PayPal credentials in `appsettings.json`.
- Never commit Client ID or Client Secret values to GitHub.
- Use Sandbox credentials for testing.

---

## 4. Database Migrations

Entity Framework Core Code First is used to manage database changes.

The migration includes:

- Enrollment Status.
- Payment table.
- Payment-to-Enrollment relationship.
- Enum string conversions.
- Payment configuration.

To create a migration from the solution directory:

```powershell
dotnet ef migrations add EnrollmentPayments --project Dalil_Academy.Infra --startup-project Dalil_Academy.API
```

Apply the migration to PostgreSQL:

```powershell
dotnet ef database update --project Dalil_Academy.Infra --startup-project Dalil_Academy.API
```

Make sure the database is created and the migrations are applied successfully before running the API.

---

## 5. Running the Project

1. Open `Dalil_Academy.sln` in Visual Studio.
2. Set `Dalil_Academy.API` as the Startup Project.
3. Verify the database connection and PayPal configuration.
4. Build the solution.
5. Run the API.
6. Open Swagger using the HTTPS URL displayed in the terminal:

```text
https://localhost:YOUR_PORT/swagger
```

---

## 6. Enrollment and Payment Workflow

The payment process follows these steps:

1. An Admin creates an enrollment for a student.
2. If the course is free, the enrollment is marked as `Paid` immediately.
3. If the course is paid, the enrollment is created with `Pending` status.
4. The Admin calls the Pay endpoint to create a PayPal Order.
5. The backend calculates the payment amount using the course price multiplied by the JOD-to-USD exchange rate (1.41).
6. The student approves the payment through the PayPal Sandbox Approval URL.
7. The Admin calls the Capture endpoint.
8. The backend captures the PayPal Order and verifies that PayPal returns `COMPLETED`.
9. The Payment status becomes `Completed`, and the Enrollment status becomes `Paid`.

If payment fails, the Payment is marked as `Failed`, while the Enrollment remains `Pending`. A new payment attempt can then be created.

PayPal Orders expire after three hours.

---

## 7. New API Endpoints

The following endpoints were added for enrollment payments and payment management.

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/api/enrollments/{id}/pay` | Creates a PayPal Order and returns the approval URL. | Admin |
| POST | `/api/enrollments/{id}/capture` | Captures the PayPal payment and updates enrollment status. | Admin |
| GET | `/api/enrollments/{id}/payments` | Retrieves payment history for an enrollment. | Admin |

### Additional Course Features

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/courses?search=net&page=1&pageSize=10` | Searches courses by title with pagination. |
| GET | `/api/courses/{courseId}/students` | Retrieves students enrolled in a course. |

Course Capacity is also supported through `MaxStudents`, which limits the number of paid enrollments.

### Existing Enrollment Endpoint

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/api/enrollments` | Creates a new enrollment. | Admin |
| GET | `/api/enrollments/me` | Retrieves the authenticated student's paid courses. | Student |

Existing API routes and CRUD functionality are preserved.

---

## 8. Payment Business Rules

- Payment amounts are calculated on the backend using the actual course price.
- The exchange rate is `1 JOD = 1.41 USD`.
- Payment currency is always USD.
- Creating a PayPal Order does not mean the payment is completed.
- A payment is successful only when PayPal returns `COMPLETED` after Capture.
- Each payment attempt is stored independently.
- Failed payments do not change the enrollment status to Paid.
- Paid enrollments cannot be deleted or moved to another course.
- Only paid enrollments count toward course capacity.
- Payment history is ordered from newest to oldest.

---

## 9. Error Handling

The API handles common errors using a global exception handler.

| HTTP Status | Description |
|---|---|
| 400 Bad Request | Invalid or failed payment, missing payment attempt, or expired order. |
| 401 Unauthorized | Missing or invalid authentication token. |
| 403 Forbidden | Insufficient permissions. |
| 404 Not Found | Enrollment or course does not exist. |
| 409 Conflict | Enrollment is already paid or course capacity is full. |
| 503 Service Unavailable | PayPal service connection failure. |

---

## 10. Testing

The API can be tested using Swagger.

Recommended testing scenarios:

1. Admin and Student login.
2. Role-based authorization.
3. Creating paid and free enrollments.
4. Creating PayPal Orders.
5. Approving payments using a Sandbox Buyer account.
6. Capturing payments and verifying `COMPLETED`.
7. Verifying Payment and Enrollment status updates.
8. Testing failed payments and retry attempts.
9. Testing three-hour order expiration.
10. Testing course capacity, course search, and payment history.
11. Testing error responses and authorization restrictions.

---

## 11. Security Notes

- JWT authentication is used to protect secured endpoints.
- Role-based authorization separates Admin and Student permissions.
- Passwords are hashed using BCrypt.
- PayPal credentials are stored using User Secrets.
- Sensitive credentials must not be included in the submitted project or repository.

---

## 12. Important References

- ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/
- Entity Framework Core: https://learn.microsoft.com/en-us/ef/core/
- PayPal Developer: https://developer.paypal.com/
- PayPal Sandbox: https://developer.paypal.com/tools/sandbox/
- PayPal Orders API: https://developer.paypal.com/docs/api/orders/v2/

---

**Project:** Dalil_Academy  
**Type:** Learning Management System (LMS)  
**Architecture:** Onion Architecture  
**Payment Provider:** PayPal Sandbox