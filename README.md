# Employee Project Allocation Portal

A centralized portal designed to simplify employee management, skill tracking, project management, and employee project allocation. The application helps organizations maintain employee records, manage project requirements, track allocation history, and monitor workforce availability through a unified dashboard.

## Overview

The Employee Project Allocation Portal provides a structured way to manage the relationship between employees and projects. It enables users to maintain employee and project information, associate skills with employees, allocate employees to active projects, and track their allocation history.

The project is being developed as a practical enterprise application with a focus on clean architecture, maintainability, data integrity, and real-world business workflows.

## Features

### 1. Employee Management
- Create, view, update, and manage employee records.
- Maintain employee details such as employee code, name, email, location, and training information.
- Prevent duplicate employee codes and email addresses.
- Protect employee records associated with allocation history from accidental deletion.

### 2. Skills Management
- Create and manage skills.
- Assign multiple skills to employees.
- View skills associated with individual employees.
- Prevent duplicate skill entries and employee-skill assignments.

### 3. Project Management
- Create, view, update, and manage projects.
- Maintain project details, client information, technology, location, and project timelines.
- Track project status: Active, Inactive, and Completed.
- Prevent deletion of projects with existing allocation history.

### 4. Employee Project Allocation
- Allocate employees to projects.
- Assign roles to employees within projects.
- Maintain allocation dates and allocator information.
- Track active and released allocations.
- Maintain complete allocation history.
- Prevent employees from having multiple active project allocations.

### 5. Dashboard
- View total employees.
- Monitor allocated and available employees.
- Track total and active projects.
- Monitor total and active allocations.

## Application Modules

| Module | Description |
|---|---|
| Employee Management | Maintain employee information |
| Skills Management | Manage skills and employee skill associations |
| Project Management | Maintain project and client information |
| Project Allocation | Allocate and release employees |
| Dashboard | View workforce and project statistics |

## Business Rules

- Every employee has a unique employee code.
- Employee email addresses must be unique.
- Skill names and project codes must be unique.
- An employee can have multiple skills.
- An employee can have only one active project allocation at a time.
- An employee can be allocated again after their previous allocation is released.
- Only active projects can receive new employee allocations.
- Allocation history is retained even after an employee is released.
- Employees and projects with allocation history cannot be deleted.

## Technology Stack

**Backend**
- ASP.NET Core Web API (.NET 10)
- C#
- Entity Framework Core
- SQL Server

**Frontend**
- Angular (planned)

**Development Tools**
- Visual Studio
- SQL Server Management Studio
- Swagger / OpenAPI
- Git and GitHub

## Project Structure

```text
EmployeeProjectAllocation/
│
├── Controllers/
│   ├── EmployeesController.cs
│   ├── SkillsController.cs
│   ├── ProjectsController.cs
│   ├── AllocationsController.cs
│   └── DashboardController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── Employees/
│   ├── Skills/
│   ├── Projects/
│   ├── Allocations/
│   └── Dashboard/
│
├── Models/
│   ├── Employee.cs
│   ├── Skill.cs
│   ├── EmployeeSkill.cs
│   ├── Project.cs
│   └── ProjectAllocation.cs
│
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│
├── Migrations/
│
├── Properties/
├── appsettings.json
├── Program.cs
└── EmployeeProjectAllocation.csproj
```

## Getting Started

### Prerequisites

Make sure the following are installed:

- .NET 10 SDK
- SQL Server LocalDB or SQL Server
- Visual Studio 2026 or Visual Studio Code
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/anirudhxmishra/employee-project-allocation.git
```

### 2. Navigate to the Project

```bash
cd employee-project-allocation
```

### 3. Configure the Database

Update the `DefaultConnection` value in `appsettings.json` according to your SQL Server configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeProjectAllocationDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 5. Run the Application

```bash
dotnet restore
dotnet build
dotnet run
```

### 6. Open Swagger

Once the application starts, open the Swagger URL displayed in the terminal. For example:

```text
https://localhost:7165/swagger
```

The port may differ depending on your local configuration.

## API Endpoints

### Employees

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Employees` | Get all employees |
| GET | `/api/Employees/{employeeCode}` | Get employee by code |
| POST | `/api/Employees` | Create employee |
| PUT | `/api/Employees/{employeeCode}` | Update employee |
| DELETE | `/api/Employees/{employeeCode}` | Delete employee |

### Skills

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Skills` | Get all skills |
| POST | `/api/Skills` | Create skill |
| POST | `/api/Skills/assign/{employeeCode}` | Assign skills to employee |
| GET | `/api/Skills/employee/{employeeCode}` | Get employee skills |

### Projects

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Projects` | Get all projects |
| GET | `/api/Projects/{id}` | Get project by ID |
| POST | `/api/Projects` | Create project |
| PUT | `/api/Projects/{id}` | Update project |
| DELETE | `/api/Projects/{id}` | Delete project |

### Allocations

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Allocations` | Get all allocations |
| GET | `/api/Allocations/{id}` | Get allocation by ID |
| POST | `/api/Allocations` | Allocate employee to project |
| GET | `/api/Allocations/employee/{employeeCode}` | Get employee allocation history |
| PUT | `/api/Allocations/release/{id}` | Release an allocation |

### Dashboard

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Dashboard` | Get dashboard statistics |

## Database Design

The application currently uses five primary entities:

- **Employees** – Stores employee information.
- **Skills** – Stores available skills.
- **EmployeeSkills** – Maintains the many-to-many relationship between employees and skills.
- **Projects** – Stores project and client information.
- **ProjectAllocations** – Maintains employee allocation records and allocation history.

Database constraints enforce important business rules, including unique employee emails, skill names, project codes, and one active allocation per employee.

## Development Roadmap

- [x] Employee Management
- [x] Skills Management
- [x] Project Management
- [x] Employee Project Allocation
- [x] Allocation Release and History
- [x] Dashboard Statistics
- [x] Service Layer Architecture
- [x] Database Constraints and Indexes
- [ ] Global Exception Handling
- [ ] Database Relationship and Performance Review
- [ ] JWT Authentication and Role-Based Authorization
- [ ] Logging and API Improvements
- [ ] Backend Testing
- [ ] Angular Frontend
- [ ] Application Deployment

## Future Enhancements

- Role-based access for administrators and HR users.
- Advanced employee and project search.
- Filtering, sorting, and pagination.
- Improved dashboard visualizations.
- Automated testing and deployment.
- A responsive frontend for easier management.

## Author

**Anirudh Mishra**

[GitHub Profile](https://github.com/anirudhxmishra)

---

*This project is developed for learning, portfolio demonstration, and exploring practical enterprise application development.*
