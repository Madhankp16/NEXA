# 🏢 NexaWork – Enterprise Management & Workflow Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-blueviolet)]()
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)]()

**NexaWork** is an Enterprise-Grade Workforce Management, Project Kanban, Multi-Level Workflow Approval, and Document Compliance System built with **.NET 8 Clean Architecture** and **React + TypeScript + Redux Toolkit**.

Designed to showcase 1-2+ years production Software Engineer experience with real-world architectural design, DDD principles, JWT token rotation, RBAC security, SignalR real-time event distribution, and Redis caching.

---

## 🏛️ Architecture Overview

The backend strictly adheres to **Clean Architecture** (Domain-Driven Design):

```
                        ┌────────────────────────┐
                        │   React 19 Frontend    │
                        │  (TypeScript + Redux)  │
                        └───────────┬────────────┘
                                    │ HTTP / SignalR
                                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                      NexaWork.Api                               │
│  - Controllers & Endpoints     - Swagger / OpenAPI              │
│  - JWT Bearer Authentication   - Global Exception Middleware    │
│  - SignalR Real-Time Hubs      - CORS Policies                  │
└───────────────┬─────────────────────────────────┬───────────────┘
                │                                 │
                ▼                                 ▼
┌───────────────────────────────┐ ┌───────────────────────────────┐
│     NexaWork.Application      │ │    NexaWork.Infrastructure    │
│  - Business DTOs & Models     │ │  - EF Core 8 DbContext        │
│  - IApplicationDbContext      │ │  - PostgreSQL Provider        │
│  - FluentValidation Rules     │ │  - BCrypt Password Hashing    │
│  - Service Abstractions       │ │  - JWT Token Generation       │
│                               │ │  - Redis Cache & MemoryCache  │
│                               │ │  - Data Seeder (DbInitializer)│
└───────────────┬───────────────┘ └───────────────┬───────────────┘
                │                                 │
                └───────────────┬─────────────────┘
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      NexaWork.Domain                            │
│  - Core Entities (Organization, User, Project, Task, Workflow)  │
│  - Domain Enums & Value Objects                                 │
│  - BaseEntity (Soft Delete & Timestamp Tracking)                │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🚀 Key Modules & Capabilities

1. **Organization & Workforce Governance**
   - Multi-tenant Organization schema with corporate Departments and Agile Squads.
   - Dynamic Roles & Permissions matrix (SuperAdmin, Manager, Employee).
   - Staff directory with designation and access tracking.

2. **Project & Kanban Board**
   - Create enterprise projects with alphanumeric keys (e.g., `NEXA`, `MOBI`).
   - 4-Column interactive Kanban board (`To Do`, `In Progress`, `In Review`, `Done`).
   - Priority tagging (`Urgent`, `High`, `Medium`, `Low`), assignee avatars, hours estimation.

3. **Multi-Level Approval Workflow Engine**
   - Governance pipeline for Budget Approvals, Leave Requests, and Expense Claims.
   - Multi-step authorization (e.g. Step 1: Department Manager &rarr; Step 2: Executive VP).
   - Reviewer decision dialogue with audit remarks.

4. **Real-Time SignalR Telemetry**
   - Websocket push alerts on task assignment, approval status transitions, and announcements.
   - Interactive notification bell with unread counters and animated pulse indicators.

5. **Document Vault & Compliance**
   - Categorized document repository (Project, HR, Finance, Legal).
   - Semantic versioning (`v1.0`, `v2.0`) and access tier classification.

6. **Executive Dashboard & Analytics**
   - Real-time KPIs: Workforce headcount, Project completion velocity, Pending approval volume.
   - Monthly sprint delivery SVG velocity charts and department distribution progress bars.

---

## 🔑 Pre-Seeded Demo Accounts

The database seeds automatically on startup with three ready-to-test personas:

| Persona | Email | Password | Role / Access |
|---|---|---|---|
| **Principal Architect** | `admin@nexawork.com` | `Password123!` | SuperAdmin (Full Access, VP Approvals) |
| **Engineering Manager** | `sarah.manager@nexawork.com` | `Password123!` | Manager (Project Oversight & Review) |
| **Full Stack Engineer** | `dev@nexawork.com` | `Password123!` | Employee (Tasks, Documents, Requester) |

> 💡 **Quick Login:** The frontend login screen includes one-click buttons to populate credentials instantly.

---

## 🛠️ How to Run Locally

### 1. Backend (.NET 8 Web API)

```bash
cd backend/NexaWork.Api
dotnet run
```
- API will start on `http://localhost:5000` (or configured port).
- Interactive Swagger UI: `http://localhost:5000/swagger`

### 2. Frontend (React + TypeScript)

```bash
cd frontend
npm install
npm run dev
```
- Client runs at: `http://localhost:5173`

### 3. Run Unit Tests (xUnit + Moq + FluentAssertions)

```bash
dotnet test backend/NexaWork.Tests/NexaWork.Tests.csproj
```

### 4. Run with Docker Compose

```bash
docker-compose up --build
```
- PostgreSQL: `localhost:5432`
- Redis: `localhost:6379`
- Backend API: `localhost:5000`
- Frontend Portal: `localhost:3000`

---

## 🎯 How to Explain This Project in Interviews

### 1. Project Summary Pitch (30 Seconds)
> *"I designed and built **NexaWork**, an enterprise workforce management and workflow automation platform. On the backend, I used **.NET 8 with Clean Architecture (DDD)**, dividing concerns into Domain, Application, Infrastructure, and API layers with **PostgreSQL via EF Core**. On the frontend, I engineered a high-performance **React 19 + TypeScript** SPA with **Redux Toolkit** and **Axios interceptors** for automatic JWT refresh token rotation. It includes interactive Kanban boards, multi-level approval workflows, and **SignalR** for real-time task telemetry."*

### 2. Deep-Dive Interview Topics

- **Why Clean Architecture?**
  - Isolates core business logic from UI frameworks and databases. If we change from PostgreSQL to SQL Server or AWS Aurora, zero domain code changes are required.
- **How is Authentication Secured?**
  - Short-lived JWT Access Tokens (120 minutes) paired with cryptographic Refresh Tokens stored in PostgreSQL. Axios response interceptors transparently intercept 401s, request a new access token, and replay failed calls without disrupting the user.
- **Handling Real-Time Events:**
  - Integrated SignalR Hubs (`/hubs/notifications`) to push instant updates to users when workflows advance or tasks are assigned.
- **Performance & Caching:**
  - Implemented Redis/In-Memory caching in the Application layer to cache expensive aggregate dashboard queries with 5-minute sliding expirations.
