# Inventory Management System

Inventory Management System (IMS) is a .NET application for managing products and creating customer orders. The project includes product listing and creation, 
order creation with stock validation, discount calculation, location charge calculation, and PostgreSQL persistence through Entity Framework Core.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- xUnit

## Project Structure

- `src/IMS.Api` - Web API, controllers, middleware and application startup.
- `src/IMS.Application` - commands, queries, DTOs, validators and application services.
- `src/IMS.Domain` - domain entities
- `src/IMS.Infrastructure` - EF Core `AppDbContext`, migrations and database seeding.
- `tests/IMS.UnitTests` - unit tests.
- `tests/IMS.IntegrationTests` - integration tests using PostgreSQL.

## Running The API

Start PostgreSQL from the root directory:

docker compose up -d

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
