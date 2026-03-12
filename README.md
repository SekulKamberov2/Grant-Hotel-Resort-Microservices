# Grant Hotel Resort Microservices Platform

A **full-stack microservices architecture** for hotel management built with modern technologies.

The platform includes:

- **React** frontend
- **Ocelot API Gateway**
- Multiple **ASP.NET Core 9 Microservices**
- **Docker Compose** orchestration
- **JWT Authentication with HttpOnly Cookies**

Everything runs locally with **one Docker command**.

---

# Quick Start Guide

Follow the steps below to **build, run, and seed the entire platform**.

---

# 1️. Clone the Repository

If you haven't already cloned the project:

```bash
git clone https://github.com/yourusername/Grant-Hotel-Resort.git
cd Grant-Hotel-Resort/GHR
```

---

# 2️. Build and Start All Containers

Run the entire microservices platform with Docker Compose:

```bash
docker-compose up --build
```

This command will:

- Build all microservice images
- Start **SQL Server**
- Start **API Gateway**
- Start all **ASP.NET Core services**
- Start the **React client**

---

# 3️. Verify SQL Server Container

Check running containers:

```bash
docker ps
```

You should see a container similar to:

```
ghr-sqlserver-1
```

---

# 4️. Copy the Database Schema Script

Copy the database schema into the SQL Server container.

```bash
docker cp full_schema.sql ghr-sqlserver-1:/tmp/full_schema.sql
```

---

# 5️. Create and Seed All Databases

Execute the schema script inside the container:

```bash
docker exec -it ghr-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd \
-S localhost \
-U sa \
-P "SekulKamberov2025!@" \
-i /tmp/full_schema.sql \
-C
```

This will:

- Create all microservice databases
- Create tables
- Seed demo data
- Prepare authentication tables

---

# 6️. Assign Roles to Users

This ensures that:

- Every user receives the **EMPLOYEE role**
- Some users receive **elevated roles**

Run the following command:

```bash
docker exec -it ghr-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd \
-S localhost \
-U sa \
-P "SekulKamberov2025!@" \
-d IdentityGHRDB \
-Q "
-- Assign EMPLOYEE role (Id = 1) to all users
INSERT INTO UserRoles (UserId, RoleId)
SELECT Id, 1 FROM Users
WHERE NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = Users.Id AND RoleId = 1
);

-- Assign HR ADMIN (Id = 3) to user sekul7 (Id = 7)
INSERT INTO UserRoles (UserId, RoleId)
SELECT 7, 3
WHERE NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = 7 AND RoleId = 3
);

-- Assign MANAGER (Id = 2) to user sekul8 (Id = 8)
INSERT INTO UserRoles (UserId, RoleId)
SELECT 8, 2
WHERE NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = 8 AND RoleId = 2
);
" -C
```

---

# 7️. Verify User Roles

Check if roles were assigned correctly.

```bash
docker exec -it ghr-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd \
-S localhost \
-U sa \
-P "SekulKamberov2025!@" \
-d IdentityGHRDB \
-Q "
SELECT 
    u.Id AS UserId,
    u.Username,
    u.Email,
    STRING_AGG(r.Name, ', ') AS Roles
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.Id
GROUP BY u.Id, u.Username, u.Email
ORDER BY u.Id;
" -C
```

You should see users with roles such as:

```
EMPLOYEE
MANAGER
HR ADMIN
```

---

# 8️. Restart Identity Service

After seeding and assigning roles, restart the identity service to clear cached errors.

```bash
docker-compose restart identity-service
```

---

# 9. You rock Access the Application now
 Open your browser and go to:

```
http://localhost:3003
```

You will see the **Sign In page**.

Use the **demo credentials from the seeded database** to log in.

---

# Architecture Overview

The system is composed of multiple microservices:

```


# Useful Docker Commands

Stop the system:

```bash
docker-compose down
```

Rebuild containers:

```bash
docker-compose up --build
```

View running containers:

```bash
docker ps
```

View logs:

```bash
docker-compose logs -f
```

---

# Security

Authentication is implemented using:

- **JWT tokens**
- Stored in **HttpOnly cookies**
- Issued by the **Identity Service**

Benefits:

- Protects against **XSS attacks**, No tokens stored in local storage, Centralized authentication via **API Gateway**

---

# Development
The platform is built using a **distributed microservices architecture** designed for scalability, maintainability, and clear domain boundaries.

It follows modern backend architecture patterns including:

- **Domain-Driven Design (DDD)**
- **CQRS (Command Query Responsibility Segregation)**
- **Mediator Pattern via MediatR**
- **API Gateway pattern using Ocelot**

---

## Backend Architecture

Each ASP.NET Core microservice is structured using **DDD + CQRS** principles.


### Key Concepts Used

**CQRS**

- Commands modify state
- Queries retrieve data
- Separation improves scalability and maintainability

**MediatR**

- Decouples controllers from business logic
- Commands and queries are processed via **handlers**

**gRPC**

- Used for **high-performance service-to-service communication**

**RabbitMQ**

– Event-driven asynchronous messaging between microservices

---

## Technology Stack

### Backend

- **ASP.NET Core 9** – Core microservice APIs implemented in C#
- **Node.js & Express** – Additional microservices for specific domains and services
- **Ocelot API Gateway** – Centralized routing and entry point for all client requests
- **gRPC** – High-performance synchronous service-to-service communication
- **RabbitMQ** – Event-driven asynchronous messaging between microservices
- **MediatR** – Implements the Mediator pattern for CQRS command and query handling
- **Dapper** – Lightweight high-performance data access for SQL Server
- **SQL Server** – Primary relational database for core services
- **MongoDB & Mongoose** – NoSQL storage used by selected Node.js services

---

## Frontend

The client application is built with:

- **React**
- **Modern component-based architecture**
- **API communication through the Ocelot Gateway**

---

## Platform Domain

The system models a **luxury resort hotel ecosystem** with multiple operational domains.

Features include management for:

- Bars
- Disco / Night Club
- Casino
- Private Beach
- Restaurants
- Fitness Center
- Spa & Wellness
- Shops & Services
- Rooms & Reservations

Each domain can be implemented as an **independent microservice**, allowing the platform to scale and evolve over time.

---

# About This Project

**Grant Hotel Resort Microservices Platform** is a portfolio project demonstrating a modern **enterprise-style microservices architecture**.

The system showcases experience with:

- **ASP.NET Core 9 Microservices**
- **API Gateway with Ocelot**
- **JWT Authentication with HttpOnly Cookies**
- **Docker & Docker Compose orchestration**
- **React frontend integration**
- **SQL Server database management**
- **Service isolation and scalable architecture**

The goal of this project is to demonstrate the design and implementation of a **realistic distributed backend system similar to those used in production environments**.

---

# Author

**Sekul Kamberov**

Software Developer

- GitHub: https://github.com/SekulKamberov2
- Youtube: https://www.youtube.com/@SMKFullStackWebDevelopment  
- LinkedIn: https://www.linkedin.com/in/sekul-kamberov  