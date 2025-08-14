# Employee Management System

A comprehensive employee and department management system built with ASP.NET Core Web API backend and React frontend, featuring CRUD operations, search, pagination, and soft delete/restore functionality.

## Live Application

- **Frontend**: [http://144.91.79.115:8080/employees](http://144.91.79.115:8080/employees)
- **Repository**: [https://github.com/nikolliervin/employee-management](https://github.com/nikolliervin/employee-management)

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ 
- SQL Server (Local or Docker)
- Docker & Docker Compose (for containerized deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd employee-management
   ```

2. **Backend Setup**
   ```bash
   cd employee-management.Server
   dotnet restore
   dotnet run
   ```
   - API will be available at: `https://localhost:7063`
   - Swagger UI: `https://localhost:7063/swagger`

3. **Frontend Setup**
   ```bash
   cd employee-management.client
   npm install
   npm run dev
   ```
   - Frontend will be available at: `http://localhost:5173`

4. **Database**
   - Update connection string in `appsettings.Development.json`
   - Run migrations: `dotnet ef database update`

### Docker Deployment

1. **Start all services**
   ```bash
   docker-compose up -d
   ```

2. **Access the application**
   - Frontend: `http://localhost:8080`
   - API: `http://localhost:8081`
   - Database: `localhost:1433`

## Features

### Core Functionality
- **Employee Management**: Create, read, update, delete employees
- **Department Management**: Create, read, update, delete departments
- **Search & Filtering**: Advanced search with multiple criteria
- **Pagination**: Page-based results with metadata
- **Soft Delete/Restore**: Data preservation with recovery capability
- **Audit Trail**: Track creation, updates, and deletions

### API Endpoints

#### Employees (`/api/v1/Employees`)
- `GET /` - List employees (paginated)
- `GET /{id}` - Get employee by ID
- `GET /search` - Search employees with filters
- `GET /deleted` - Get deleted employees
- `POST /` - Create new employee
- `PUT /{id}` - Update employee
- `DELETE /{id}` - Soft delete employee
- `POST /{id}/restore` - Restore deleted employee

#### Departments (`/api/v1/Departments`)
- `GET /` - List departments (paginated)
- `GET /{id}` - Get department by ID
- `GET /search` - Search departments with filters
- `GET /deleted` - Get deleted departments
- `POST /` - Create new department
- `PUT /{id}` - Update department
- `DELETE /{id}` - Soft delete department
- `POST /{id}/restore` - Restore deleted department

## Architecture

### Backend Architecture
- **ASP.NET Core 8.0** Web API
- **Clean Architecture** with layered approach
- **Repository Pattern** for data access
- **Service Layer** for business logic
- **DTO Pattern** for data transfer
- **AutoMapper** for object mapping
- **Entity Framework Core** with SQL Server

### Frontend Architecture
- **React 18** with modern hooks
- **Vite** for fast development and building
- **Context API** for state management
- **React Router** for navigation
- **Axios** for HTTP requests
- **Tailwind CSS** for styling

### Data Layer
- **SQL Server** database
- **Entity Framework Core** ORM
- **Code-First** approach with migrations
- **Soft Delete** implementation
- **Audit Trail** tracking
- **Optimistic Concurrency** control

## Infrastructure

### Hosting Decision
This application is hosted on an **Ubuntu server** for full control over container orchestration and cost-effective hosting for containerized applications.

### Development Environment
- **Local Development**: .NET, Node.js, SQL Server
- **Hot Reload**: Both frontend and backend support hot reload
- **Swagger Documentation**: Auto-generated API documentation

### Production Deployment
- **Ubuntu Server**: Hosted on a dedicated Ubuntu server for full container control
- **Docker Containers**: All services containerized with Docker Compose
- **Nginx**: Reverse proxy for frontend serving and load balancing
- **Multi-stage Builds**: Optimized production images for minimal size
- **Health Checks**: Database and service health monitoring
- **Environment Configuration**: Flexible configuration management via environment variables
- **Port Mapping**: Strategic port allocation (8080 for frontend, 8081 for API, 1433 for database)

### Container Architecture
```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                                    Docker Network                                  │
│                              employee-network (bridge)                             │
└─────────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │
                    ┌───────────────────┼───────────────────┐
                    │                   │                   │
                    ▼                   ▼                   ▼
        ┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────┐
        │   Frontend Client   │ │   Backend API       │ │   SQL Server       │
        │   Container         │ │   Container         │ │   Container         │
        │                     │ │                     │ │                     │
        │ ┌─────────────────┐ │ │ ┌─────────────────┐ │ │ ┌─────────────────┐ │
        │ │   Nginx         │ │ │ │ ASP.NET Core    │ │ │ │ SQL Server      │ │
        │ │   (Port 3000)   │ │ │ │ (Port 8080)     │ │ │ │ (Port 1433)     │ │
        │ │                 │ │ │ │                 │ │ │ │                 │ │
        │ │ • Static Files  │ │ │ │ • Controllers   │ │ │ │ • Database      │ │
        │ │ • React App     │ │ │ │ • Services      │ │ │ │ • Migrations    │ │
        │ │ • Routing       │ │ │ │ • Repositories  │ │ │ │ • Health Checks │ │
        │ └─────────────────┘ │ │ │ • Middleware    │ │ │ └─────────────────┘ │
        └─────────────────────┘ │ │ • Exception     │ │ └─────────────────────┘
                                │ │   Handling      │ │
                                │ └─────────────────┘ │
                                └─────────────────────┘
                                        │
                                        │
                    ┌───────────────────┼───────────────────┐
                    │                   │                   │
                    ▼                   ▼                   ▼
        ┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────┐
        │   Host Ports        │ │   Internal Ports    │ │   Data Persistence │
        │                     │ │                     │ │                     │
        │ • Frontend: 8080   │ │ • Frontend: 3000   │ │ • SQL Data Volume  │
        │ • API: 8081        │ │ • API: 8080        │ │ • Migration Files  │
        │ • Database: 1433   │ │ • Database: 1433   │ │ • Log Files        │
        └─────────────────────┘ └─────────────────────┘ └─────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────────┐
│                                    Data Flow                                      │
│                                                                                     │
│  Client Request → Nginx (Port 8080) → React App → API Call → Backend (Port 8081)   │
│                                                                                     │
│  Backend → Entity Framework → SQL Server (Port 1433) → Database Response           │
│                                                                                     │
│  Response → Backend → React App → Nginx → Client                                   │
└─────────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────────┐
│                                  Health Monitoring                                │
│                                                                                     │
│  • Database Health Check: SQL connectivity validation                             │
│  • API Health Check: Endpoint availability                                        │
│  • Container Health Check: Process status monitoring                              │
│  • Network Health Check: Inter-service communication                              │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

## Project Structure

```
employee-management/
├── employee-management.Server/          # Backend API
│   ├── Controllers/                    # API endpoints
│   ├── Services/                       # Business logic
│   ├── Repositories/                   # Data access
│   ├── Models/                         # DTOs, entities, responses
│   ├── Data/                           # DbContext and migrations
│   └── Extensions/                     # Service configurations
├── employee-management.client/          # Frontend React app
│   ├── src/
│   │   ├── components/                 # React components
│   │   ├── services/                   # API service calls
│   │   ├── context/                    # State management
│   │   └── types/                      # TypeScript definitions
│   └── public/                         # Static assets
├── docker-compose.yml                  # Docker services
└── README.md                           # This file
```

## Configuration

### Environment Variables
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)
- `VITE_API_URL`: Frontend API base URL

### Database Configuration
- **SQL Server**: Primary database
- **Migrations**: Auto-applied on startup
- **Connection Pooling**: Optimized for performance
- **Health Checks**: Database connectivity monitoring

## Deployment

### Docker Compose
```bash
# Production deployment
docker-compose -f docker-compose.yml up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Manual Deployment
1. Build backend: `dotnet publish -c Release`
2. Build frontend: `npm run build`
3. Deploy to web server
4. Configure environment variables
5. Run database migrations

## API Documentation

- **Swagger UI**: Available at `/swagger` when running locally
- **OpenAPI Specification**: Auto-generated from code
- **Examples**: Request/response examples for all endpoints
- **Validation Rules**: Field constraints and business rules
- **Error Codes**: Comprehensive error handling documentation

## Testing

- **Unit Tests**: Service and repository layer testing
- **Integration Tests**: API endpoint testing
- **Test Coverage**: Comprehensive test coverage for business logic
