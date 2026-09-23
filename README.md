# Wholesale SaaS

A multi-tenant foundation for a wholesale management platform, built on **Clean Architecture** and **CQRS**.

This is an architectural exploration of the same domain as [toptan-satis](https://github.com/furkanmar/toptan-satis): how the product would look as a properly layered, multi-tenant SaaS.

## Architecture

```
src/
  Domain/          Entities (Tenant, User) — no dependencies
  Application/     Use cases as MediatR commands/queries, validation, auth
  Infrastructure/  EF Core + PostgreSQL, identity, external services
  Web/             ASP.NET Core API
  AppHost/         .NET Aspire orchestration
tests/
  Domain.UnitTests · Application.UnitTests
  Application.FunctionalTests · Infrastructure.IntegrationTests
```

- Started from [Jason Taylor's Clean Architecture template](https://github.com/jasontaylordev/CleanArchitecture) and adapted for multi-tenancy
- JWT access + refresh token authentication
- Docker Compose setup for PostgreSQL and the API

## Running

```bash
dotnet run --project src/AppHost     # opens the Aspire dashboard
# or
docker compose up -d --build
```

## Status

Early stage — tenant and auth foundation is in place; domain features are next.
