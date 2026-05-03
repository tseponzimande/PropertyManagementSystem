# Property Management System (PMS)

A modern, full-stack **Property Management System** built with **ASP.NET Core**, **Blazor**, and **Entity Framework Core** to streamline property operations for admins, owners, and tenants.

This project is designed to digitize rental property workflows — from lease management and rent tracking to maintenance requests, communication, and reporting — all within a secure role-based platform.


## Features

### Role-Based Access Control
- **Admin** – full system access and oversight
- **Owner** – manage owned properties, units, leases, and tenants
- **Tenant** – self-service portal for payments, maintenance, and communication

---

### Core Modules

#### Property & Unit Management
- Add, edit, and manage properties
- Unit allocation and tracking
- Ownership-based visibility controls

#### Lease Management
- Create and manage lease agreements
- Track active and past leases
- Lease termination workflows

#### Payment Tracking
- Submit and record rental payments
- Payment history per tenant
- Financial export functionality

####  Maintenance Requests
- Create and track maintenance tickets
- Approval and closure workflows
- Request status monitoring

#### Real-Time Communication
- Integrated chat system
- WhatsApp-style message display
- Role-based messaging access

####  Notifications
- In-app notification center
- Read/unread filtering
- Mark all as read support

#### Reporting & Analytics
- Financial summaries
- Export to CSV
- Dashboard insights by role

---

## Tech Stack

### Frontend
- **Blazor Server**
- **Radzen UI Components**

### Backend
- **ASP.NET Core**
- **C#**

### Data & Security
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Identity**
- **JWT Authentication**

### Architecture
- **Clean Architecture**
- **Service Layer Pattern**
- **DTO-based communication**
- **Dependency Injection**

---

## Project Structure
PropertyManagementSystem/
│
├── Application/        # Business logic, DTOs, services
├── Domain/             # Core entities
├── Infrastructure/     # Database, repositories, integrations
├── UI/                 # Blazor components and pages
└── API/                # Endpoints / startup configuration
