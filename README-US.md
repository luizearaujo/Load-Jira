## 1. Application Overview

[Back](README.md)

LoadJira is a console application developed in C# (.NET) designed to extract issue data (tasks, bugs, stories, etc.) from Jira Cloud via REST API. After extraction, the data is processed and persisted in a SQL Server database. The application can be run on-demand to load historical data and calculate essential agile metrics such as Lead Time and Cycle Time, in addition to managing Story Points.

## 2. Application Architecture

The LoadJira architecture is modular, organized into logical layers to ensure separation of concerns, maintainability, and testability. The main layers are:

### 2.1. LoadJira.Console (Execution Layer)

-   **Responsibility:** Acts as the application's entry point. It is responsible for interpreting command-line arguments, initializing the logging system (Serilog), and orchestrating the execution of business services.
-   **Technologies:** C#, Serilog.

### 2.2. LoadJira.Domain (Business Layer)

-   **Responsibility:** Contains the core business logic of the application. Services in this layer coordinate operations, using repositories for data access and API services for external communication. This is where business rules for issue processing, Story Point calculation, and time metrics are applied.
-   **Technologies:** C#, Serilog.

### 2.3. LoadJira.Infra (Infrastructure Layer)

-   **Responsibility:** Abstracts the implementation details of data access (SQL Server database) and external API communication (Jira REST API). It includes repositories for entity persistence and services for interacting with third-party APIs.
-   **Technologies:** C#, Dapper (ORM), HttpClient, Newtonsoft.Json, Serilog.

### 2.4. LoadJira.Entities (Entities Layer)

-   **Responsibility:** Defines the data models (POCOs - Plain Old C# Objects) that represent the application's business entities (e.g., Issue, Person, Project, Status, Detail). These entities are used across all layers to ensure data consistency.
-   **Technologies:** C#.

### 2.5. LoadJira.Config (Configuration Layer)

-   **Responsibility:** Stores global application settings such as Jira access credentials (user, token), Jira API URL, and SQL Server connection string.
-   **Technologies:** C#.

## 3. Class Structure and Responsibilities

### 3.1. Service Classes (LoadJira.Domain)

-   **`BaseService.cs`:** Abstract base class for business services, providing `ILogger` injection and common repositories.
-   **`IssueService.cs`:** Orchestrates the loading and processing of Jira issues, interacting with the Jira API and various repositories for data persistence and updates.
-   **`StoryPointService.cs`:** Calculates and persists Story Points (first and last value) for issues, analyzing the history of details.
-   **`TimeService.cs`:** Calculates and persists Lead Time and Cycle Time for issues, based on recorded status changes.

### 3.2. Infrastructure Classes (LoadJira.Infra)

-   **`JiraWebApiService.cs`:** Manages asynchronous communication with the Jira REST API to fetch issues, details, and changelogs, including authentication and JSON deserialization.
-   **`BaseRepository.cs`:** Generic abstract class for data access, responsible for managing the secure opening and closing of SQL connections and providing abstract methods for save operations.
-   **Specific Repositories (`IssueRepository.cs`, `DetailRepository.cs`, `PersonRepository.cs`, `ProjectRepository.cs`, `StatusRepository.cs`, `TypeRepository.cs`):** Inherit from `BaseRepository<T>` and implement CRUD (Create, Read, Update, Delete) logic for their respective entities using Dapper.

### 3.3. Entities (LoadJira.Entities)

-   **`Issue.cs`:** Represents a Jira issue, including metadata, processing status, and fields for calculated Story Points and times.
-   **`Detail.cs`:** Represents an item from an issue's changelog, detailing field changes like status or Story Points.
-   **`Person.cs`:** Represents a Jira user (e.g., reporter, change author).
-   **`Project.cs`:** Represents a Jira project.
-   **`Status.cs`:** Represents a Jira workflow status.
-   **`Type.cs`:** Represents a Jira issue type (e.g., Story, Bug, Task).

### 3.4. Mapping (LoadJira.Infra.Mapping)

-   **`IssueMapping.cs`:** Maps objects from the Jira API to the `Issue` domain entities.
-   **`IssueDetailMapping.cs`:** Maps changelog data from the Jira API to the `Detail` domain entities.

## 4. Database

The LoadJira application uses a SQL Server database to persist data extracted from Jira. The structure is relational, optimized for storing and querying issue data and its metrics.

### 4.1. Main Tables

Tables are created and managed by SQL scripts located in `LoadJira.Infra/Repository/sql/`:

-   **`Type`:** Issue types (Id, Name).
-   **`Project`:** Jira projects (Key, Name).
-   **`Status`:** Issue statuses (Id, Name).

-   **`Person`:** People (Id, Name).
-   **`Issue`:** Jira issues (Key, Summary, Description, Created, Processed, StoryPointProcessed, TimeProcessed, FirstStoryPoint, LastStoryPoint, LeadTime, CycleTime, and foreign keys for Type, Status, Project, Person).
-   **`Detail`:** Changelog details (Id, Created, Type, From, To, IssueKey (FK), AuthorId (FK)).

### 4.2. Data Access

Database access is performed by the repositories in the `LoadJira.Infra` layer. Dapper is used as a Micro-ORM to execute SQL commands and map results to C# objects. Database operations are encapsulated within repository methods, which use connections managed by the `BaseRepository`.

## 5. Processes and Transformations

The LoadJira application executes different processes based on the command provided on the command line (`issue`, `sp`, `time`).

### 5.1. `issue` Process

Responsible for the initial extraction and loading of Jira issue data:

1.  **Issue Key Loading:** Optionally, fetches new issue keys from the Jira API (`JiraWebApiService.GetKeysAsync`) up to a specified end date.
2.  **Initial Persistence:** New issue keys are saved to the local database (`IssueRepository.Save(issuesKey)`).
3.  **Selection for Processing:** Identifies issues in the local database that have not yet been fully processed.
4.  **Individual Processing:** For each unprocessed issue:
    -   Retrieves full issue details and its changelog from the Jira API (`JiraWebApiService.GetIssueAsync`, `GetDetailsAsync`).
    -   Saves or updates related entities (Type, Project, Status, Reporter) and changelog details in the database.
    -   Marks the issue as processed in the database upon success.

### 5.2. `sp` Process (Story Points)

Calculates and persists Story Points for issues:

1.  **Issue Selection:** Fetches issues not yet processed for Story Points from the database.
2.  **Detail Retrieval:** Retrieves the issue's changelog from the local database.
3.  **Calculation:** Analyzes the changelog to identify the first and last Story Point values.
4.  **Persistence:** Updates the issue in the database with the calculated Story Points and marks it as SP processed.

### 5.3. `time` Process (Lead Time and Cycle Time)

Calculates and persists Lead Time and Cycle Time for issues:

1.  **Issue Selection:** Fetches issues not yet processed for time metrics from the database.
2.  **Detail Retrieval:** Retrieves the issue's changelog from the local database.
3.  **Lead Time Calculation:** Calculates the difference in days between the issue's creation date and the date of the last transition to a "Done" or "Concluído" status (by analyzing status change details).
4.  **Cycle Time Calculation:** Calculates the difference in days between the first transition to an "In Progress" (or similar) status and the last transition to "Done" or "Concluído" (by analyzing status change details).
5.  **Persistence:** Updates the issue in the database with the calculated times and marks it as time processed.

## 6. Application Configuration

The `LoadJira.Config/Config.cs` file is crucial for the application's operation and **must be reviewed and updated before execution**. It stores sensitive and global configurations.

### 6.1. Essential Parameters

The following values need to be adjusted:

-   **`_jiraUser`:** Jira account username (usually email).
-   **`_jiraToken`:** API token for authentication with Jira Cloud.
-   **`_jiraUrl`:** Base URL of the Jira Cloud instance (e.g., `https://your-domain.atlassian.net`).
-   **`_sqlServerConn`:** SQL Server connection string.

**Configuration Example (`Config.cs`):**

```csharp
private static string _jiraUser => "your-email@example.com";
private static string _jiraToken => "your-jira-api-token";
private static string _jiraUrl => "https://your-domain.atlassian.net";
private static string _sqlServerConn => @"Server=your-sql-server;Database=LoadJira;User Id=your-user;Password=your-password;";
```

### 6.2. Important Configuration Notes

-   **Mandatory Update:** Default values are placeholders and **must be replaced** with your actual data.
-   **Security:** In production environments, consider loading credentials from environment variables or secure vaults for enhanced security.

## 7. Database Schema and Object Creation

Before running the application, the database must be created and configured. SQL scripts to define the schema and necessary objects are available in `LoadJira.Infra/Repository/sql/`.

### 7.1. Script Execution Order

The scripts must be executed in the following sequence:

1.  `01_type_create_table.sql`
2.  `02_project_create_table.sql`
3.  `03_status_create_table.sql`
4.  `04_person_create_table.sql`
5.  `05_issue_create_table.sql`
6.  `06_detail_create_table.sql`
7.  `07_issue_alter_table_sp.sql`
8.  `08_issue_alter_table_time.sql`

### 7.2. Important Database Notes

-   **Database:** The scripts assume the use of the `jira_database`. Ensure this database exists on your SQL Server.
-   **Integrity:** Tables are created with primary and foreign keys to ensure referential integrity.
-   **Execution:** It is recommended to execute the scripts using a tool like SQL Server Management Studio (SSMS), verifying the success of each step.

---
[Back](README.md)