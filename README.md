# Authentication & Authorization API

A simple ASP.NET Core Web API built with C#, .NET 10, PostgreSQL, and Entity Framework Core.

The project supports user registration, login, JWT authentication, and role-based authorization using User and Admin roles.

## 1. Technologies Used

- ASP.NET Core Web API (.NET 10)
- C#
- PostgreSQL
- Entity Framework Core
- JWT Bearer Authentication
- Swagger
- Postman

## 2. Database Setup

Start PostgreSQL:

```bash
sudo service postgresql start
```

Create the database:

```sql
CREATE DATABASE auth_task;
```

Create a `.env` file in the project directory:

```env
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=auth_task;Username=postgres;Password=YOUR_PASSWORD"

Jwt__Key="YOUR_RANDOM_JWT_SECRET"

Admin__FullName="System Administrator"
Admin__Email="admin@authtask.local"
Admin__Password="YOUR_ADMIN_PASSWORD"
```

Use a secure random JWT key of at least 32 bytes and an Admin password of at least 12 characters.

Apply the database migrations:

```bash
dotnet ef database update
```

The database stores user information, password hashes, and roles.

## 3. Running the Application

Restore packages and build:

```bash
dotnet restore
dotnet build
```

Run the API:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://127.0.0.1:5020
```

Swagger:

http://127.0.0.1:5020/swagger

## 4. API Endpoints

| Method | Endpoint | Access |
|---|---|---|
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |
| GET | /api/profile | Authenticated |
| GET | /api/user | User / Admin |
| GET | /api/admin | Admin only |

## 5. Postman Testing

The exported Postman collection is located at:

`postman/AuthTask.postman_collection.json`

To run the tests:

1. Start the API.
2. Open Postman and import the collection.
3. Run the registration and login requests.
4. Use the JWT tokens to test protected endpoints.
5. Check the automated test results.

The tests cover successful registration and login, invalid credentials, valid and invalid JWTs, unauthorized access (401), forbidden access (403), and duplicate registration (400).

Use a new email when repeating registration tests. Login again when a JWT expires.

## 6. Key Design Decisions

- **Password Hashing:** Uses ASP.NET Core PasswordHasher to store passwords securely instead of plain text.
- **JWT Authentication:** Generates signed tokens containing user identity and role claims.
- **Role-Based Authorization:** Uses `[Authorize]` to protect endpoints based on User and Admin roles.
- **Middleware:** `UseAuthentication()` validates JWTs, while `UseAuthorization()` checks permissions.
- **Database:** Uses EF Core migrations and a unique email constraint.
- **DTOs:** Separate registration and login requests from database entities.
- **Admin Account:** Created through AdminSeeder in Development instead of public registration.
- **Swagger:** Supports Bearer token authentication for testing protected endpoints.


