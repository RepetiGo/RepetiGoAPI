# Documentation for the backend project

## Run project
1. Using Visual Studio
2. Using the .NET CLI:
```bash
# Run the project
dotnet run
# Run the project in https profile
dotnet run --launch-profile https
# Run the project in hot reload mode
dotnet watch run
```

## Update database
1. Using the Package Manager Console in Visual Studio:
```powershell
# Create a new migration
Add-Migration [name-of-migration] -OutputDir Data\Migrations
# Apply the migration to the database
Update-Database
```
2. Using the .NET CLI:
```bash
# Create a new migration
dotnet ef migrations add [name-of-migration] --output-dir Data\Migrations
# Apply the migration to the database
dotnet ef database update
```