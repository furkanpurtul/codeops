# CodeOps

CodeOps is a .NET 10 template for building opinionated, layered backend services. The repository already contains a runnable ASP.NET Core API shell, versioned OpenAPI support, a custom mediator implementation, DDD-oriented domain primitives, and PostgreSQL infrastructure scaffolding.

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## What This Template Gives You

- A thin API host with controllers, problem details, health checks, HTTPS redirection, API versioning, and OpenAPI/Swagger.
- A custom mediator stack that discovers commands, queries, notifications, and pipeline behaviors by scanning loaded assemblies.
- Domain building blocks for aggregates, entities, value objects, strongly typed IDs, enumerations, repositories, and domain violations.
- Infrastructure scaffolding for Entity Framework Core with PostgreSQL and domain event dispatch during `SaveChangesAsync`.
- A hosting layer that restricts runtime environments to `Development`, `Test`, `Staging`, and `Production`.

## Current State Of The Template

This repository is a starter, not a finished application.

- The API currently exposes one sample controller: `GET /api/v1/examples`.
- Health endpoints are mapped at `/health/live` and `/health/ready`.
- Versioned OpenAPI JSON and Swagger UI are available outside production.
- `CodeOps.Application` contains abstractions and behaviors, but its `AddApplication` extension is still empty.
- PostgreSQL support exists in `CodeOps.Infrastructure.EntityFrameworkCore.Npgsql`, but the API host does not yet call `AddNpgsql`, so database-backed behavior is scaffolded rather than fully wired.

## Solution Structure

### `src/CodeOps.Api`

The presentation layer. It owns HTTP concerns such as controllers, API versioning, OpenAPI registration, health checks, and middleware ordering.

### `src/CodeOps.Application`

The use-case layer. It defines messaging contracts such as `ICommand`, `IQuery`, handlers, publishers, and result types. Cross-cutting request behaviors live here.

### `src/CodeOps.Domain`

The core domain model. It provides reusable DDD primitives such as aggregate roots, entities, value objects, strongly typed IDs, enumerations, repositories, and domain violation helpers.

### `src/CodeOps.Infrastructure.Mediator`

The in-process messaging implementation. It scans assemblies for handlers and pipeline behaviors, enforces single request-handler registration, and supports pluggable publish strategies.

### `src/CodeOps.Infrastructure.EntityFrameworkCore.Npgsql`

The database infrastructure. It contains `ApplicationDbContext`, Npgsql options, EF Core setup, and domain-event dispatch integration tied to persistence.

### `src/CodeOps.Hosting`

Shared hosting primitives. It validates allowed environments and centralizes option registration helpers used by the other projects.

## How To Run The Template

### Prerequisites

- .NET 10 SDK
- A PostgreSQL instance if you plan to wire the readiness check and persistence layer

### Build

```powershell
dotnet build .\CodeOps.slnx
```

### Run The API

```powershell
dotnet run --project .\src\CodeOps.Api\CodeOps.Api.csproj --launch-profile Development
```

The development launch profile uses `https://localhost:5000` and opens Swagger at `/swagger`.

### Available Environments

The host validates environment names and currently accepts only:

- `Development`
- `Test`
- `Staging`
- `Production`

If you add deployment-specific configuration, keep those names aligned with the hosting layer.

## Current HTTP Endpoints

### Health

- `GET /health/live`
- `GET /health/ready`

### Sample API

- `GET /api/v1/examples`

The API versioning stack also recognizes:

- URL segment versioning, for example `/api/v1/examples`
- Header versioning via `x-api-version`
- Query string versioning via `api-version`

### OpenAPI

Outside production, the API exposes:

- Swagger UI at `/swagger`
- Versioned OpenAPI documents at `/openapi/{group}.json`, for example `/openapi/v1.json`

## Core Concepts In This Template

### 1. Layered Architecture

Dependencies flow inward.

- API depends on application contracts and infrastructure composition.
- Application defines use-case abstractions and result handling.
- Domain stays focused on business concepts and invariants.
- Infrastructure implements technical concerns such as mediator dispatch and persistence.

This keeps HTTP, persistence, and domain logic separated so you can replace infrastructure without rewriting core behavior.

### 2. Command And Query Messaging

The application layer models use cases as commands and queries.

- Commands implement `ICommand` or `ICommand<TResponse>`.
- Queries implement `IQuery<TResponse>`.
- Handlers implement matching handler interfaces.
- Pipeline behaviors can wrap requests for validation, logging, transactions, or exception translation.

The mediator infrastructure discovers handlers automatically from loaded assemblies, so adding a new feature is usually a matter of adding a request type and a handler.

### 3. Result-Based Application Responses

The `Result` and `Result<T>` types encode success and failure without forcing exception-driven flow for expected outcomes. This is useful when mapping domain or validation failures into predictable API responses.

### 4. Domain-Driven Building Blocks

The domain project provides reusable primitives for tactical DDD patterns.

- `AggregateRoot<TId>` tracks domain events and optimistic concurrency version.
- `Entity<TId>` gives identity-based equality.
- `ValueObject<TDerived>` supports structural equality.
- `StronglyTypedId<TValue>` reduces primitive obsession.
- `Enumeration<TEnumeration>` supports rich enum-like behavior.

Use these when you start modeling real business concepts instead of leaving the template at the transport layer.

### 5. Domain Violations

The domain layer includes an `Ensure` pipeline for expressing business rule failures explicitly and collecting them before throwing.

- `Ensure.For<TSource>()` starts a violation context for a domain type or operation.
- `Guard(...)` records unexpected violations for low-level invariants and unsafe input.
- `Validate(...)` records validation violations for business-facing validation errors.
- `Check(...)` evaluates `IRule` and `IRule<TContext>` implementations.
- `OrThrow()` raises a `DomainViolationException` containing a structured violation list.

Each violation carries a `ViolationKind`, member name, message, and source information. The current kinds are:

- `Unexpected`
- `Forbidden`
- `Validation`
- `Conflict`

In the application layer, `DomainViolationBehavior<TRequest, TResponse>` catches `DomainViolationException` and maps the collected violations into typed application errors through `DomainViolationErrorMapper`. That keeps domain failures explicit without leaking raw exceptions directly to the API surface.

Example shape:

```csharp
Ensure.For<Order>()
    .Validate(customerId, x =>
    {
        if (x.Value is null)
            x.AddViolation("Customer id is required.");
    })
    .Check(new OrderMustHaveAtLeastOneLineRule(lines))
    .OrThrow();
```

### 6. Persistence And Domain Events

`ApplicationDbContext` acts as the unit of work. During `SaveChangesAsync`, it can publish domain events through `IPublisher` before committing. This gives you a clear extension point for keeping side effects aligned with aggregate changes.

## How To Extend The Template

### Add A New Feature

1. Define a command or query in the application layer.
2. Implement its handler in an assembly loaded by the API host.
3. Add domain types or rules in the domain layer as needed.
4. Expose the use case from a controller in `CodeOps.Api`.
5. If the feature requires persistence, register the PostgreSQL infrastructure and add EF Core mappings.

### Wire PostgreSQL

To make the persistence stack active, add the Npgsql registration in the API host and configure the `Npgsql` section in appsettings or environment variables.

Expected settings:

- `Npgsql:Host`
- `Npgsql:Port`
- `Npgsql:Database`
- `Npgsql:Username`
- `Npgsql:Password`

After that, create migrations from the infrastructure project and use the API project as the startup project.

Example commands:

```powershell
dotnet ef migrations add InitialCreate --project .\src\CodeOps.Infrastructure.EntityFrameworkCore.Npgsql --startup-project .\src\CodeOps.Api
dotnet ef database update --project .\src\CodeOps.Infrastructure.EntityFrameworkCore.Npgsql --startup-project .\src\CodeOps.Api
```

## Suggested First Steps After Cloning

1. Rename the sample `ExamplesController` to your first real feature area.
2. Implement `AddApplication` to register validators, feature services, and other application-level dependencies.
3. Decide whether you want the custom mediator implementation to remain in-process only or to integrate with external messaging.
4. Wire PostgreSQL if the service needs persistence and readiness checks tied to the database.
5. Add tests around handlers, domain rules, and API behavior before growing the template.

## Development Notes

- Swagger and OpenAPI are disabled in production by design.
- The launch profiles all bind to `https://localhost:5000`.
- The `.http` file under `src/CodeOps.Api` has been updated to reflect the current endpoints.

## License

MIT. See [LICENSE](LICENSE).
