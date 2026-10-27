
# AISmokingDetection - Run Instructions

## Requirements
- Visual Studio
- .NET SDK compatible with the project
- SQL Server LocalDB

## Database
Database name: AISmokingDetectionDB

Server: (localdb)\MSSQLLocalDB

## Run the Application
1. Open AISmokingDetection.sln in Visual Studio.
2. Set AISmokingDetection as the startup project.
3. Ensure the database connection string is configured in appsettings.json.
4. Build the solution.
5. Run the application using Ctrl + F5.

## Available Endpoints
- GET /health
- GET /api/Staff

## Database Setup
Entity Framework Core is used to manage the database.

The initial migration was created using:
Add-Migration InitialCreate

The database was updated using:
Update-Database
