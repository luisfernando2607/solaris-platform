![.NET](https://img.shields.io/badge/.NET-10-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Architecture](https://img.shields.io/badge/Clean--Architecture-✔-green)
![License](https://img.shields.io/badge/License-Proprietary-red)

# 🏢 Solaris Platform -- Backend

## 📌 Overview

**Solaris Platform** is an enterprise-grade ERP backend designed for
medium-sized companies.

Built with **.NET 10 LTS**, **Clean Architecture**, and **PostgreSQL
16**, the platform is optimized for scalability, maintainability,
multi-tenancy, and on-premise deployments.E

------------------------------------------------------------------------

## 🧱 Architecture

The project follows modern software engineering principles:

-   Clean Architecture\
-   Domain-Driven Design (DDD)\
-   RESTful API\
-   JWT Authentication\
-   Multi-tenancy ready\
-   Modular ERP structure\
-   Separation of concerns\
-   Dependency inversion

------------------------------------------------------------------------

## ⚙️ Tech Stack

-   .NET 10 LTS\
-   ASP.NET Core\
-   Entity Framework Core 10\
-   PostgreSQL 16\
-   FluentValidation\
-   AutoMapper\
-   Serilog (structured logging)\
-   Docker\
-   Redis (optional caching layer)

------------------------------------------------------------------------

## 📂 Project Structure

``` bash
src/
 ├── SolarisPlatform.Domain          # Core business rules and entities
 ├── SolarisPlatform.Application     # Use cases and application logic
 ├── SolarisPlatform.Infrastructure  # Data access and external services
 ├── SolarisPlatform.API             # REST API entry point
 └── SolarisPlatform.Tests           # Unit and integration tests
```

------------------------------------------------------------------------

## 🚀 Getting Started

### 1️⃣ Restore Packages

``` bash
dotnet restore
```

### 2️⃣ Apply Database Migrations

``` bash
dotnet ef database update
```

### 3️⃣ Run the Project

``` bash
dotnet run --project src/SolarisPlatform.API
```

The API will start using the configured environment settings.

------------------------------------------------------------------------

## 🔐 Environment Variables

Example configuration:

``` env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=solar_db;Username=postgres;Password=your_password
Jwt__Key=your_super_secret_key
Jwt__Issuer=SolarisPlatform
Jwt__Audience=SolarisPlatformUsers
```

> ⚠️ Never commit sensitive credentials to source control.

------------------------------------------------------------------------

## 🧪 Testing

Run unit and integration tests with:

``` bash
dotnet test
```

------------------------------------------------------------------------

## 📈 Roadmap

-   [ ] Security Module\
-   [ ] Full Multi-tenancy Implementation\
-   [ ] Audit Logging\
-   [ ] Notification System\
-   [ ] Financial ERP Modules\
-   [ ] Role & Permission Management\
-   [ ] CI/CD Pipeline

------------------------------------------------------------------------

## 🐳 Docker Support (Optional)

Build and run with Docker:

``` bash
docker build -t solaris-backend .
docker run -p 5000:80 solaris-backend
```

------------------------------------------------------------------------

## 👨‍💻 Author

**Luis Fernando Flores Cuadros**\
Founder -- Solaris Platform

------------------------------------------------------------------------

## 📄 License

Proprietary -- All rights reserved.
