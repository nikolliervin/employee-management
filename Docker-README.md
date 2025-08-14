# Employee Management Docker Setup

This Docker setup runs the ASP.NET Core API server, React client, and SQL Server database in containers with proper networking.

## Architecture

- **Frontend (React)**: Runs on port 8080 (accessible via browser)
- **Backend (ASP.NET Core API)**: Runs internally on port 8081, proxied through nginx
- **Database (SQL Server)**: Runs on port 1433 with data persisted in Docker volume

## Quick Start

### Prerequisites
- Docker
- Docker Compose

### Running the Application

1. **Build and start all services:**
   ```bash
   docker-compose up --build
   ```

2. **Run in background (detached mode):**
   ```bash
   docker-compose up -d --build
   ```

3. **Access the application:**
   - Frontend: http://localhost:8080
   - API (direct): http://localhost:8081
   - SQL Server: localhost:1433 (sa/TestPass123!)

### Useful Commands

**Stop all services:**
```bash
docker-compose down
```

**Stop and remove volumes (clears database):**
```bash
docker-compose down -v
```

**View logs:**
```bash
docker-compose logs
```

**View logs for specific service:**
```bash
docker-compose logs sql-server
docker-compose logs employee-api
docker-compose logs employee-client
```

**Rebuild only one service:**
```bash
docker-compose up --build employee-api
docker-compose up --build employee-client
```

## Network Configuration

- All services run in a custom Docker network (`employee-network`)
- The React app uses nginx as a proxy to forward API calls to the backend
- API calls from the frontend go through nginx proxy at `/api/*` routes
- SQL Server database is persisted using Docker volumes
- API service waits for SQL Server to be healthy before starting
- Entity Framework migrations run automatically on container startup

## Development Notes

- The API server runs in Production mode inside the container
- SQL Server database is persisted in the `sql-data` Docker volume
- Frontend is served by nginx with proper caching headers
- CORS is handled by nginx proxy configuration
- SQL Server uses SA authentication with password: `TestPass123!`
- Database migrations are applied automatically on API container startup

## Database Access

You can connect to the SQL Server database directly using:
- **Server**: localhost,1433
- **Username**: sa
- **Password**: TestPass123!
- **Database**: EmployeeManagement

Use SQL Server Management Studio (SSMS) or Azure Data Studio to connect.

## Troubleshooting

1. **Port conflicts**: If ports 8080, 8081, or 1433 are already in use, modify the ports in `docker-compose.yml`
2. **Database issues**: Remove the volume with `docker-compose down -v` to reset the SQL Server database
3. **Build cache issues**: Use `docker-compose build --no-cache` to rebuild from scratch
4. **SQL Server startup**: The database container may take 30-60 seconds to fully initialize on first run
5. **Migration errors**: Check API logs with `docker-compose logs employee-api` if migrations fail

## Security Notes

**Important**: The default SA password `TestPass123!` is for development only. 
For production deployment:
1. Change the SA password in the `docker-compose.yml`
2. Update the connection string accordingly
3. Consider using Azure SQL Database or a managed SQL Server instance
4. Implement proper secret management