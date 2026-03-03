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
├── NexOrder.UserService.Messages      # Integration message contracts
├── NexOrder.UserService.Shared        # Shared utilities & common models
```

---

## 🚀 Features

- Create, update, and manage users
- Retrieve user profiles and details
- Clean separation of concerns across layers
- Designed for scalability and extensibility
- Secured behind Azure API Management
- Event publication for downstream services

---

## 🛠️ Tech Stack

- **.NET 8**
- **Azure Functions**
- **Entity Framework Core**
- **MediatR**
- **Azure SQL**
- **Azure API Management**
- **Azure Service Bus**
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

## 📣 Event-Driven Messaging

NexOrder.UserService participates in an **event-driven architecture** using **Azure Service Bus** for asynchronous communication between microservices.

### 🔄 Message Publishing

When a user is updated, the service publishes a domain event to Azure Service Bus:

- **Topic:** `userserviceevents`
- **Event Type:** `UserUpdated`
- **Message Contract Library:** `NexOrder.UserService.Messages`

This enables other services (e.g., Order Service, Inventory Service) to react to user changes without tight coupling.

---

### 🧾 Message Contract

Message contracts are defined in a dedicated shared library:

```
NexOrder.UserService.Messages
└── UserUpdated
```

Benefits:
- Strongly typed event contracts
- Clear ownership of integration boundaries
- Easy versioning and reuse across services

---

### 📐 Event Flow (User Update)

1. Client updates a user via API
2. UserService persists changes using EF Core
3. `UserUpdated` event is published to Service Bus topic
4. Downstream services consume the event asynchronously

---

### 🧠 Design Rationale

- Improves scalability and resilience
- Enables future consumers without modifying User Service
- Mirrors real-world distributed system design

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

## 🐳 Docker Support

This service can be run locally using **Docker** and **Docker Compose**.

### Prerequisites

- Docker Desktop (or Docker Engine)
- Docker Compose v2

### 🧱 Dockerfile

A `Dockerfile` is included to build a container image for the service.

Build an image locally:

```bash
docker build -t nexorder-userservice:local .
```

Run the container (example):

```bash
docker run --rm -p 8080:80 \
  -e ConnectionStrings__SystemDbConnectionString="<connection-string>" \
  -e ConnectionStrings__ServiceBusConnectionString="<servicebus-connection-string>" \
  nexorder-productservice:local
```

> Note: Actual port bindings and hosting settings depend on how the Function host is configured in the container.
> 

### 🧩 Docker Compose

A `docker-compose.yml` is included to simplify local orchestration.

Start services:

```bash
docker compose up --build
```

Stop services:

```bash
docker compose down
```

### 🔐 Configuration in Containers

For local containers, prefer **environment variables** (or a local `.env` file referenced by Compose) rather than committing secrets.

Common keys:

- `ConnectionStrings__SystemDbConnectionString`
- `ConnectionStrings__ServiceBusConnectionString`

---

## 🚢 Deployment

### GitHub Actions

The service supports two deployment workflows using **GitHub Actions** with Azure:

1. **Standard deployment (without containerization)** — builds and deploys the Function App directly
2. **Containerized deployment** — builds a Docker image, pushes to Azure Container Registry, and deploys to Azure Web App for Containers

> **Currently, only the containerized deployment workflow is enabled.**
> 

### Standard Deployment Workflow (Disabled)

When enabled, this workflow:

- Builds & restores the .NET project
- Applies EF Core migrations (controlled pipeline step)
- Deploys directly to Azure Functions

> API Management instances are recreated on demand for cost optimization in non-production environments.
> 

### 🧊 Containerized Deployment Workflow (Active)

This service is deployed as a container to **Azure Web App for Containers**.

High-level flow:

1. Build the Docker image via GitHub Actions
2. Push image to **Azure Container Registry**
3. Configure Azure Web App for Containers to pull and run the image
4. Provide required configuration via **App Settings** (environment variables)

Recommended App Settings (examples):

- `ConnectionStrings__SystemDbConnectionString`
- `ConnectionStrings__ServiceBusConnectionString`
- Any other runtime configuration used by the Function host

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

## 📌 Notes

- Business functionality is intentionally minimal
- Focus is on architecture, security, and cloud integration
- Designed to be consumed by any frontend or service

---
