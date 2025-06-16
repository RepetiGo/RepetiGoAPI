# Documentation for the Flashcard.Api project

## Run project
1. Using Visual Studio (2022 or later):

   - Open the solution file (`.sln`).
   - Set the desired launch profile (e.g., `https` or `http`) in the toolbar.
   - Press `F5` to run the project with debugging, or `Ctrl + F5` to run without debugging.

2. Using the .NET CLI:

	- Run the project `dotnet run`
	- Run the project in https profile `dotnet run --launch-profile https`
	- Run the project in hot reload mode `dotnet watch`

## Update database
1. Using the Package Manager Console in Visual Studio:

	- Create a new migration
	```powershell
	Add-Migration [name-of-migration] -OutputDir Data\Migrations
	```

	- Apply the migration to the database
	```powershell
	Update-Database
	```
2. Using the .NET CLI:

    - Create a new migration
    ```bash
    dotnet ef migrations add [name-of-migration] --output-dir Data\Migrations
    ```
    - Apply the migration to the database
    ```bash
    dotnet ef database update
    ```

## Todo
- [ ] Fix filters and sorting
- [ ] Add request logging middleware
- [ ] Add Retry policy for API endpoints
- [ ] Add moving cards between decks functionality
- [ ] Fix logic of reviewing cards