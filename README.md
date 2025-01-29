# Urban System Project Setup Guide

## Prerequisites

- JetBrains Rider
- .NET SDK 8.0
- Docker Desktop
- Git

## Docker SQL Server Setup

1. Create a new directory for your SQL Server container:

```bash
mkdir windowsSqlServerDocker
cd windowsSqlServerDocker
```

2. Create a `Dockerfile`:

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/mssql/server:2022-latest

# Environment variables for SQL Server
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=Admin123
ENV MSSQL_PID=Express
ENV MSSQL_ENCRYPT=OPTIONAL

# Create directories
RUN mkdir -p /var/opt/mssql/data
RUN mkdir -p /var/opt/mssql/backup
```

3. Create a `docker-compose.yml`:

```yaml
# docker-compose.yml
version: "3.8"

services:
  sqlserver:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: sqlserver
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql/data
      - sqlbackup:/var/opt/mssql/backup
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=Admin123
      - MSSQL_PID=Express
      - MSSQL_ENCRYPT=OPTIONAL
    restart: unless-stopped

volumes:
  sqldata:
    driver: local
  sqlbackup:
    driver: local
```

4. Start SQL Server container:

```bash
docker-compose up -d
```

## Project Setup

1. Clone the repository:

```bash
git clone [your-repository-url]
cd "CSharp Urban System Sept 2024"
```

2. Install .NET SDK 8.0 if not already installed:

```bash
brew install dotnet@8
```

3. Add .NET to your PATH in `~/.zshrc`:

```bash
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
```

4. Install Entity Framework tools:

```bash
dotnet tool install --global dotnet-ef
```

5. Add the .NET tools to your PATH:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

## Database Configuration

1. Update your connection string in `UrbanSystem.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=YourDatabaseName;User Id=sa;Password=Admin123;TrustServerCertificate=True;"
  }
}
```

2. Navigate to the Data project:

```bash
cd UrbanSystem.Data
```

3. Run database migrations:

```bash
dotnet ef migrations add Initial --project . --startup-project ../UrbanSystem.Web
dotnet ef database update --project . --startup-project ../UrbanSystem.Web
```

## Running the Project

1. Open the solution in Rider:

   - Launch Rider
   - File -> Open -> Navigate to your project folder
   - Select the `.sln` file

2. Set `UrbanSystem.Web` as the startup project

3. Run the project using the play button in Rider or press ⌘+Enter

## Troubleshooting

If you encounter Entity Framework related issues:

1. Make sure your SQL Server container is running:

```bash
docker ps
```

2. If you need to remove migrations:

```bash
dotnet ef migrations remove --project . --startup-project ../UrbanSystem.Web
```

3. If you need to reset the database:

- Drop all connections
- Remove the existing migration
- Add a new migration
- Update the database

## Additional Notes

- The project uses .NET 8.0 and Entity Framework Core
- Initial data seeding includes Bulgarian cities and sample suggestions
- The SQL Server instance runs on localhost:1433
- Default admin credentials are in appsettings.json
