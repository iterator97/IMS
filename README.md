# Inventory Management System

Inventory Management System (IMS) is a .NET application for managing products and creating customer orders. The project includes product listing and creation, 
order creation with stock validation, discount calculation, location charge calculation, and PostgreSQL persistence through Entity Framework Core.

Reuirements covered:
- Product listing and creation with stock management.
- Order creation with stock validation, discount calculation, and location charge calculation.
- * Discount dates setup covered in IMS.Infrastructure/DataInitializer.cs [BlackFriday, Holiday]. With ability to ability to add feature with new discount, setup discount mode, disable/enable discount.

## Tech Stack
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- xUnit
- Docker

## Project Structure

- `src/IMS.Api` - Web API, controllers, middleware and application startup.
- `src/IMS.Application` - commands, queries, DTOs, validators and application services.
- `src/IMS.Domain` - domain entities
- `src/IMS.Infrastructure` - EF Core `AppDbContext`, migrations and database seeding.
- `tests/IMS.UnitTests` - unit tests.
- `tests/IMS.IntegrationTests` - integration tests using PostgreSQL.

## Docker Setup
Setup the .env file for the API container in `src/IMS.Api/.env` with content like:
```
POSTGRES_DB=products_db
POSTGRES_USER=username
POSTGRES_PASSWORD=password
```

Run docker compose from the repository root to start the PostgreSQL container for the API:
docker compose up -d


## Running The API - Development
Setup the databese connection string in `src/IMS.Api/appsettings.Development.json`, sample below:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=products_db;Username=username;Password=password"
  }
}
```

docker compose up -d

Turn off the API container to run it locally:

Run the API:

dotnet run --project src/IMS.Api

Swagger is available when the API is running:

http://localhost:7001/swagger


## Integration Tests

Integration tests use a separate PostgreSQL container on port `5433`.

Create `tests/IMS.IntegrationTests/.env` with content like:

Host=localhost;Port=5433;Database=ims_integration_tests;Username=username;Password=password

Start the integration test database:

cd tests/IMS.IntegrationTests
docker compose up -d

Run all tests from the repository root:

dotnet test .\IMS.slnx
