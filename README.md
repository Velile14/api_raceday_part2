# api_raceday_part2

# RaceDay API

## Project Overview

RaceDay is an event management API that allows organisers to manage events and participants to enrol in events and track their results. The API uses role-based authorisation to restrict access to protected operations.

## Requirements

- .NET 8 SDK
- Visual Studio 2022 or another .NET-compatible editor
- SQL Server
- Git

## Setup and Run Instructions

1. Clone or download this repository.
2. Open the solution in Visual Studio 2022.
3. Configure the SQL Server connection string in `appsettings.json` to match your local database.
4. Open a terminal at the repository root and restore the dependencies:

   ```bash
   dotnet restore
   ```

5. Build the solution:

   ```bash
   dotnet build --configuration Release
   ```

6. Run the unit tests:

   ```bash
   dotnet test --configuration Release
   ```

7. Run the API:

   ```bash
   dotnet run --project api_raceday_part2/api_raceday_part2.csproj
   ```

   Open the Swagger URL displayed in the terminal if Swagger is enabled in the current environment.

## User Roles

### Organiser

- Create, update and delete their events.
- Manage event categories.
- Capture participant results.
- View enrolments for their events.

### Participant

- Register an account and log in.
- Browse available events.
- Enrol in an event using an available category.
- View their own enrolments.
- Track their personal results.

Access to protected API operations is controlled through authentication and role-based authorisation.

## Continuous Integration — GitHub Actions

The RaceDay CI workflow runs automatically when changes are pushed to the repository or a pull request is created. It restores dependencies, builds the solution and runs the unit tests.

### Successful Build Evidence

Insert a screenshot of the successful workflow run from the GitHub Actions tab below. The screenshot should show the workflow name and a green checkmark confirming that the build and tests passed.

![GitHub Actions successful build screenshot](docs/screenshots/github-actions-green-build.png)
