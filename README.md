# Asset Tracking System

A console application for tracking company assets across multiple offices, built with .NET 8 and Entity Framework Core.

## Features

- Asset management (CRUD operations)
- Employee management
- Office management with multi-currency support
- Maintenance records tracking
- Asset lifecycle status (Green/Yellow/Red)
- Role-based access (Admin, Manager, Employee)
- Dashboard with statistics
- Reports generation

## Tech Stack

- .NET 8
- Entity Framework Core
- SQL Server
- Repository Pattern

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or Docker)

### Database Setup

1. Start SQL Server using Docker:
```bash
docker-compose up -d
```

2. Apply migrations:
```bash
cd TrackingSystem.App
dotnet ef database update
```

### Run the Application

```bash
dotnet run --project TrackingSystem.App
```

### Default Users

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Admin |
| manager | manager123 | Manager |
| employee | employee123 | Employee |

## Project Structure

```
TrackingSystem/
├── TrackingSystem.App/
│   ├── Data/           # DbContext and migrations
│   ├── Models/         # Entity models
│   ├── Repositories/   # Data access layer
│   ├── Services/       # Business logic
│   └── UI/             # Console interface
└── docker-compose.yml
```

## License

MIT
