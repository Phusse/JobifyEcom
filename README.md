# Jobify

A job marketplace platform built with **ASP.NET Core** using Clean Architecture and a microservices-based approach.

## Overview

Jobify is an e-commerce platform for job listings and applications. The architecture consists of:

- **Jobify.Api** – The API Gateway that handles authentication, session management, and routes requests to downstream microservices via YARP reverse proxy.
- **Jobify.Ecom.Api** – The E-commerce service responsible for job listings and job applications.

Each service follows **Clean Architecture** with separate layers:
| Layer | Purpose |
|-------|---------|
| **Domain** | Core entities and business logic |
| **Application** | Use cases, CQRS handlers, and DTOs |
| **Infrastructure** | External service integrations |
| **Persistence** | Database access and EF Core configurations |

---

## Requirements

- .NET 10 SDK
- SQL Server (or compatible database)
- Redis (for session caching)

---

## Configuration

Before running, update `appsettings.json` in each API project with your environment values:

### Jobify.Api (`src/Jobify.Api/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Database": "Server=<YOUR_DB_SERVER>;Database=JobifyDb;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True;",
    "Redis": "<YOUR_REDIS_HOST>:<PORT>"
  },
  "ReverseProxy": {
    "Clusters": {
      "ecom": {
        "Destinations": {
          "d1": { "Address": "http://localhost:5243" }
        }
      }
    }
  }
}
```

### Jobify.Ecom.Api (`src/Jobify.Ecom.Api/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Database": "Server=<YOUR_DB_SERVER>;Database=JobifyDb;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True;"
  }
}
```

---

## Running the Backend

### Option 1: Run All Services (Recommended)

Open two terminal windows and run each service:

**Terminal 1 – API Gateway:**
```bash
cd src/Jobify.Api
dotnet run
```

**Terminal 2 – Ecom Service:**
```bash
cd src/Jobify.Ecom.Api
dotnet run
```

### Option 2: Run from Solution Root

```bash
# Run the Gateway
dotnet run --project src/Jobify.Api

# Run the Ecom Service (in another terminal)
dotnet run --project src/Jobify.Ecom.Api
```

## Database Migrations

Jobify uses **Entity Framework Core** for database migrations. To create or update the database schema, run migrations from the **solution root** using the `dotnet ef` CLI.

### Prerequisites

* Make sure the **.NET 10 SDK** is installed.
* Ensure your `appsettings.json` files are correctly configured with the connection strings.
* Install EF Core CLI tools if not already installed:

```bash
dotnet tool install --global dotnet-ef
```

You can check the installation:

```bash
dotnet ef --version
```

---

### Run Migrations from Solution Root

#### 1. Apply Migrations for **Jobify.Api** (Gateway)

```bash
dotnet ef database update --project src/Jobify.Persistence --startup-project src/Jobify.Api
```

* `--project` points to the **EF Core DbContext project** (Persistence layer)
* `--startup-project` points to the **project that runs the application** (Gateway)

#### 2. Apply Migrations for **Jobify.Ecom.Api** (E-commerce Service)

```bash
dotnet ef database update --project src/Jobify.Ecom.Persistence --startup-project src/Jobify.Ecom.Api
```

> **Tip:** If you need to **add a new migration**, use:

```bash
dotnet ef migrations add <MigrationName> --project <PersistenceProject> --startup-project <ApiProject>
```

Example:

```bash
dotnet ef migrations add InitialCreate --project src/Jobify.Ecom.Persistence --startup-project src/Jobify.Ecom.Api
```

---

### Verification

After running the migrations, check that the database tables have been created in your Database. You should see tables corresponding to:

* Users, Jobs, Applications, etc. (depending on the DbContext models)
* EF Core migration history table: `__EFMigrationsHistory`

---

## Service Ports

| Service | HTTP | HTTPS |
|---------|------|-------|
| **Jobify.Api** (Gateway) | `http://localhost:5177` | `https://localhost:7053` |
| **Jobify.Ecom.Api** | `http://localhost:5243` | `https://localhost:7120` |

---

## Testing the API

### Access via the Gateway (Recommended)

All requests should be routed through the **API Gateway** at `http://localhost:5177`. The gateway handles authentication, session management, and proxies requests to downstream services.

**Example requests:**

```bash
# Jobify check (Gateway)
curl http://localhost:5177/api

# E-commerce endpoints (proxied via Gateway)
curl http://localhost:5177/api/ecom/jobs
curl http://localhost:5177/api/ecom/applications
```

### API Route Mapping

The Gateway proxies requests based on these routes:

| Route Pattern | Target Service |
|---------------|----------------|
| `/api/ecom/**` | Jobify.Ecom.Api |
| `/api/matching/**` | Matching Service |
| `/api/matching-ai/**` | AI Matching Service |
| `/api/roadmap/**` | Roadmap Service |

### API Documentation (Scalar)

In development mode, interactive API documentation is available:

- **Gateway API Docs:** `http://localhost:5177/scalar/v1`
- **Ecom API Docs:** `http://localhost:5243/scalar/v1`

---

## Project Structure

```
jobify-e_com/
├── src/
│   ├── Jobify.Api/              # API Gateway
│   ├── Jobify.Application/      # Gateway application layer
│   ├── Jobify.Domain/           # Gateway domain layer
│   ├── Jobify.Infrastructure/   # Gateway infrastructure
│   ├── Jobify.Persistence/      # Gateway persistence
│   │
│   ├── Jobify.Ecom.Api/         # E-commerce API service
│   ├── Jobify.Ecom.Application/ # Ecom application layer
│   ├── Jobify.Ecom.Domain/      # Ecom domain layer
│   ├── Jobify.Ecom.Infrastructure/
│   └── Jobify.Ecom.Persistence/
│
├── Jobify.slnx                  # Solution file
├── Directory.Build.props        # Shared build properties
└── Directory.Packages.props     # Central package management
```

---

## License

See [LICENSE](LICENSE) for details.
