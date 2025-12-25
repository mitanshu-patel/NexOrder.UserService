# NexOrder.UserService

This repository contains the **User Service** microservice for the NexOrder platform — a cloud-native .NET microservices solution built using Clean Architecture principles and Azure services.

---

## 🧱 Overview

NexOrder.UserService is responsible for **user management and profile-related operations** within the NexOrder ecosystem.  
The service intentionally keeps business functionality simple (CRUD-style user operations) while demonstrating **real-world backend architecture, cloud-native patterns, security, CI/CD, and inter-service communication**.

The primary goal of this project is to showcase **senior-level backend engineering practices**, not feature-heavy business logic.

---

## 🧩 Key Concepts Demonstrated

- Clean Architecture (Domain / Application / Infrastructure)
- Azure Functions (serverless microservice)
- MediatR (CQRS-style command/query separation)
- Entity Framework Core
- Azure SQL Database
- Azure API Management (API Gateway)
- JWT-based authentication (validated at API-M)
- Inter-service HTTP communication
- GitHub Actions CI/CD
- Cloud-ready configuration & secrets handling

---

## 📁 Project Structure

```
NexOrder.UserService
├── NexOrder.UserService               # Azure Functions host
├── NexOrder.UserService.Domain        # Domain entities & business rules
├── NexOrder.UserService.Application   # Use cases, handlers, interfaces
├── NexOrder.UserService.Infrastructure# EF Core, DB context, migrations
├── NexOrder.UserService.Shared        # Shared utilities & common models
```

---

## 🚀 Features

- Create, update, and manage users
- Retrieve user profiles and details
- Clean separation of concerns across layers
- Designed for scalability and extensibility
- Secured behind Azure API Management

---

## 🛠️ Tech Stack

- **.NET 8**
- **Azure Functions**
- **Entity Framework Core**
- **MediatR**
- **Azure SQL**
- **Azure API Management**
- **GitHub Actions**

---

## 🔄 Inter-Service Communication

NexOrder.UserService supports **synchronous communication** with other microservices via HTTP APIs.

### 🔗 Usage Scenario

The service can be consumed by:
- Authentication Service (for profile enrichment)
- Order Service (for user-related validations)
- Frontend applications via API Management

All inbound requests are routed and secured through **Azure API Management**, ensuring centralized authentication and policy enforcement.

---

## ⚙️ Local Development

### Prerequisites

- .NET SDK 8+
- Azure Functions Core Tools
- SQL Server (local or Azure)
- dotnet-ef CLI

---

### Restore Dependencies

```bash
dotnet restore
```

---

## ⚙️ Application Configuration

### appsettings.json

``` json
{
  "ConnectionStrings": {
    "SystemDbConnectionString": "<Azure SQL Connection String>",
    "ServiceBusConnectionString": "<Azure Service Bus Connection String>",
  },
  "APIM_BASE_URL": "https://api.nexorder.com/auth"
  }
}
```

---

### Apply EF Core Migrations

```bash
dotnet ef database update \
  --project NexOrder.UserService.Infrastructure \
  --startup-project NexOrder.UserService.Infrastructure
```

---

### Run Locally

```bash
func start
```

---

## 🔐 Security & Authentication

- Authentication is handled by a dedicated **Auth Service**
- JWT tokens are validated at **Azure API Management**
- User Service assumes authenticated requests from API-M
- No authentication logic is embedded inside the microservice

---

------------------------------------------------------------------------

## 🌐 API Management Integration

- API is added to API Management by referencing the deployed Azure Function App.
- Inbound policy includes CORS configuration.
- `validate-jwt` inbound policy enforced
- API Management becomes the only entry point for clients consuming this authentication service.

------------------------------------------------------------------------

## API Endpoints (Sample)

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /users/search | Search users |
| GET | /users/{id} | Get user by ID |
| POST | /users | Create new user |
| PUT | /users/{id} | Update user |
| DELETE | /users/{id} | Delete user |

---

## 🚢 Deployment

The service is deployed using **GitHub Actions** and Azure services:

- Build & restore
- Apply EF Core migrations (controlled pipeline step)
- Deploy to Azure Function App
- Secured and exposed via Azure API Management

> API Management instances are recreated on demand for cost optimization in non-production environments.

---

## 📌 Notes

- Business functionality is intentionally minimal
- Focus is on architecture, security, and cloud integration
- Designed to be consumed by any frontend or service

---
