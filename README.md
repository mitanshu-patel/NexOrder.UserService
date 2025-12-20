# NexOrder.UserService

NexOrder.UserService is a core microservice in the **NexOrder**
ecosystem responsible for **user management**.\
It is implemented using **Clean Architecture** and mirrors the
structure, depth, and operational practices of **NexOrder.AuthService**.

------------------------------------------------------------------------

## 🎯 Service Responsibilities

-   User profile management
-   Ownership of user-related database schema
-   Secure integration with AuthService
-   Designed for cloud-native deployment

------------------------------------------------------------------------

## 🏗 Clean Architecture Overview

The service strictly follows Clean Architecture, ensuring separation of
concerns and testability.

    NexOrder.UserService
    │
    ├── .github/
    │ └── workflows/
    │ └── (CI/CD pipelines for GitHub Actions)
    │
    ├── NexOrder.UserService.Application/
    │ └── Application layer (use cases, commands, queries, DTOs)
    │
    ├── NexOrder.UserService.Domain/
    │ └── Domain models, entities, enums, value objects
    │
    ├── NexOrder.UserService.Infrastructure/
    │ └── Persistence, Repositories, Dependency injection, external integrations
    │
    ├── NexOrder.UserService.Shared/
    │ └── Shared utilities, constants, errors, common helpers
    │
    ├── NexOrder.UserService/
    │ └── API project (Controllers, Startup / Program, Filters, Middleware)
    │
    ├── NexOrder.UserService.sln
    ├── .gitignore

------------------------------------------------------------------------

## 🔐 Authentication & Authorization

-   JWT Bearer authentication
-   Token validation enforced via **Azure API Management**
-   No token issuance in this service

> Authentication responsibility is delegated entirely to
> **NexOrder.AuthService**.

------------------------------------------------------------------------

## 🛡 Security Restrictions

-   API access only via **API Management**
-   Direct public access disabled
-   JWT validation through inbound APIM policy
-   Database access restricted via Managed Identity / AAD
-   Principle of least privilege enforced

------------------------------------------------------------------------

## 🗄 Database & Persistence

-   Entity Framework Core (Code-First)
-   Azure SQL with AAD authentication
-   Service-owned schema
-   Migration-based schema evolution

### Core Entities

-   `User`

------------------------------------------------------------------------

## ⚙️ Application Configuration

### appsettings.json

``` json
{
  "ConnectionStrings": {
    "SystemDbConnectionString": "<Azure SQL Connection String>"
  },
  "APIM_BASE_URL": "https://api.nexorder.com/auth"
  }
}
```

------------------------------------------------------------------------

## 🌐 API Management Integration

-   All requests routed via **Azure API Management**
-   `validate-jwt` inbound policy enforced
-   Rate limiting & throttling supported
-   Centralized logging and monitoring

------------------------------------------------------------------------

## 🚀 Running Locally

### Prerequisites

-   .NET SDK 8+
-   SQL Server / Azure SQL
-   EF Core CLI
-   Visual Studio / VS Code

### Run Service

``` bash
dotnet restore
dotnet build
dotnet run --project NexOrder.UserService.API
```

------------------------------------------------------------------------

## 🗄 Applying Database Migrations

``` bash
dotnet ef database update \
  --project NexOrder.UserService.Infrastructure \
  --startup-project NexOrder.UserService
```

------------------------------------------------------------------------

## 🚢 Deployment Strategy

-   Azure App Service
-   Uses Managed Identity
-   Secrets stored in Azure Key Vault
-   CI/CD via GitHub Actions

------------------------------------------------------------------------

## 🔄 Inter-Service Communication

-   HTTP-based communication
-   Secure calls using APIM
-   Auth validation delegated to AuthService
-   Designed for future event-driven architecture

------------------------------------------------------------------------

> Part of the **NexOrder Microservices Platform**
