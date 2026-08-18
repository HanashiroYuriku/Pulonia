# Pulonia - Network Pinger
A network pinger based on [Columbina Template](https://github.com/HanashiroYuriku/Columbina)

## Technical Specifications

* **Language:** [C# .NET](https://learn.microsoft.com/en-us/dotnet/)
* **Web Framework:** [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0)
* **Database:** MySQL
* **ORM:** [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) (via Pomelo.EntityFrameworkCore.MySql)
* **Architecture & Pattern:** Clean Architecture & CQRS via [Cortex.Mediator](https://medium.com/@eneshoxha_65350/cortex-mediator-a-free-open-source-alternative-to-mediatr-for-cqrs-in-net-59534e1305c7)
* **Configuration:** Native .NET appsettings.json
* **Validation:** [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
* **Security & Auth:** JWT Bearer & BCrypt.Net
* **Logging & Monitoring:** Serilog & ASP.NET Core HealthChecks

## Core Features
1. **Architecture:** Implementation of Clean Architecture combined with Vertical Slice Architecture (feature-oriented development).
2. **Separation of Concerns (CQRS):** Application of the Command and Query Responsibility Segregation pattern using the Cortex.Mediator library to strictly separate data modification operations (`Commands`) from data retrieval operations (`Queries`).
3. **Data Validation:** Utilization of FluentValidation integrated into Cortex.Mediator Pipeline Behaviors to ensure all validation processes are executed automatically and centrally.
4. **Centralized Error Handling:** Implementation of a Global Exception Handler compliant with the RFC 7807 international standard (Problem Details) to generate uniform and informative error response formats.
5. **Database Management:** Utilization of the Entity Framework Core Object-Relational Mapping (ORM) alongside the Pomelo MySQL data provider, supported by database schema definitions via Fluent API configurations.
6. **Design Pattern:** Application of the Repository Pattern, serving as an abstraction layer to isolate business logic from direct data access operations.
7. **System Security:** Securing credentials and sensitive data through cryptographic hashing mechanisms using the BCrypt.Net-Next library.
8. **Activity Logging:** Implementation of a comprehensive logging system encompassing inbound request logging, as well as behavior monitoring for every Command and Query execution.
9. **Upcoming Developments:** Planned integration of component testing (Unit Testing), container-based infrastructure management utilizing Docker, and response format standardization for data pagination.

## Template Structure
```text
Pulonia/
    ├── Pulonia.sln
    ├── src/
    │   │
    │   ├── Pulonia.Domain/                        # [Domain Layer]
    │   │   ├── Common/                              # BaseEntity.cs (Now using Guid. You can change to UUIDNext, ULID or UUIDv7 [.NET 9+] to optimize performance)
    │   │   ├── Entities/                            # User.cs (Pure Domain/Database Models without dependencies)
    │   │   ├── Enums/                               # RoleEnum.cs
    │   │   ├── Exceptions/                          # Domain-specific custom exceptions under PuloniaException class(e.g., NotFoundException, UnauthorizedException)
    │   │   └── Pulonia.Domain.csproj
    │   │
    │   ├── Pulonia.Application/                   # [Application Layer: Use Cases (CQRS & Vertical Slicing)]
    │   │   ├── Common/                              
    │   │   │   ├── Behaviors/                       # Cortex.Mediator Pipeline Behaviors (e.g., ValidationBehavior, LoggingBehavior)
    │   │   │   ├── Interfaces/                      
    │   │   │   │   ├── Authentication/              # IPasswordHasher.cs, IJwtProvider.cs
    │   │   │   │   ├── Caching/                     # ICacheService.cs (Not Implemented)
    │   │   │   │   ├── ExternalServices/            # External Services
    │   │   │   │   └── Repositories/                # IUserRepository.cs
    │   │   │   └── Models/                          # ApiResponse.cs, PaginatedList.cs
    │   │   │
    │   │   ├── Features/
    │   │   │   └── Auth/                            # All Use Cases related to Auth
    │   │   │       └── Commands/                    # Login and Refresh Token                            
    │   │   │   └── Users/                           # All Use Cases related to Users
    │   │   │       ├── Commands/                    # Request DTO / Command Object, Business logic handler, Command FluentValidation rules
    │   │   │       │   └── CreateUser.cs            # e.g., for Commands
    │   │   │       └── Queries/                     # Response DTO / Query Object, Business logic handler              
    │   │   │           └── GetUserById              # e.g., for Queries
    │   │   │
    │   │   ├── DependencyInjection.cs               # Cortex.Mediator & FluentValidation Service Registration
    │   │   └── Pulonia.Application.csproj
    │   │
    │   ├── Pulonia.Infrastructure/                # [Infrastructure Layer: External Services, Database, & Tools]
    │   │   ├── Authentication/                      # BcryptPasswordHasher.cs, JwtProvider.cs (Access and Refresh Token)
    │   │   ├── Caching/                             # RedisCacheService.cs or other caching services (Not implemented)
    │   │   ├── Data/                                
    │   │   │   ├── Configurations/                  # EF Core Entity Configurations
    │   │   │   ├── Repositories/                    # UserRepository.cs (IUserRepository Implementation)
    │   │   │   ├── DbTransactionManager.cs          # Database Transaction Manager
    │   │   │   └── ApplicationDbContext.cs          # Main EF Core DbContext
    │   │   ├── ExternalServices/                    # EmailService.cs
    │   │   ├── Logging/                             # Custom Logging like Elasticsearch and Custom Serilog
    │   │   ├── Migration/                           # Auto-generated EF Core Migrations
    │   │   ├── Pulonia.Infrastructure.csproj
    │   │   └── DependencyInjection.cs               # DB, Redis, and Infrastructure implementations registration
    │   │
    │   ├── Pulonia.API/                             # [Presentation Layer / Entry Point]
    │   │   ├── Controllers/v1/                      # UsersController.cs
    │   │   ├── Extensions/                          # SwaggerSetup.cs, SerilogSetup.cs, AuthSetup.cs, CorsSetup.cs, VersioningSetup.cs, HealthCheck.cs
    │   │   ├── Health/                              # Database, Cache, Server, and others unit health check
    │   │   ├── Middlewares/                         # RequestLoggingMiddleware.cs, JwtMiddleware.cs
    │   │   ├── Properties/                          # launchSettings.json
    │   │   ├── Services/                          #
    │   │   ├── logs/                                # Logs result
    │   │   ├── appsettings.json                     # Main configuration (Connection strings, Logging levels)
    │   │   ├── appsettings.Development.json         # Development environment overrides
    │   │   └── Program.cs                           # Composition Root: Dependency Injection & Middleware Pipeline
    │   │
    │   └── Pulonia.Worker/                          # [Woker]
    │       ├── BackgroundServices/            # 
    │       │   └── Worker.cs        # 
    │       │
    │       ├── Extensions/            # 
    │       │   └── SerilogSetup.cs
    │       │
    │       ├── Services/            # 
    │       │   └── SystemUserService.cs
    │       │
    │       ├── logs/            # 
    │       │  
    │       ├── Properties/
    │       │   └── launchSettings.json        # 
    │       │
    │       ├── appsettings.json               # 
    │       ├── appsettings.Development.json
    │       ├── Pulonia.Worker.csproj
    │       └── Program.cs                     # 
    ├── tests/
    │   ├── Pulonia.UnitTests/                     # Tests for Domain & Application logic (No I/O dependencies)
    │   ├── Pulonia.IntegrationTests/              # Tests for API Endpoints & actual Database integration
    │   └── Pulonia.ArchitectureTests/             # NetArchTest (Ensures architectural boundaries are not violated)
    ├── .gitignore
    ├── global.json
    ├── .template.config
    ├── LICENSE
    └── README.md
```

## Dependency
````mermaid
graph TD
    API[Pulonia.API<br>Presentation / Entry Point] --> App[Pulonia.Application<br>Use Cases / CQRS]
    API --> Infrastructure[Pulonia.Infrastructure<br>External Services / DB]
    Worker[Pulonia.Worker<br>Background Services] --> App[Pulonia.Application<br>Use Cases / CQRS]
    Worker --> Infrastructure[Pulonia.Infrastructure<br>External Services / DB]
    Infrastructure --> App
    App --> Domain[Pulonia.Domain<br>Entities / Core Logic]
    Infrastructure --> Domain

    %% Custom Colors with Black Text (color:#333) for visibility
    style Domain fill:#ffd966,stroke:#333,stroke-width:2px,color:#333
    style App fill:#f4cccc,stroke:#333,stroke-width:2px,color:#333
    style Infrastructure fill:#cfe2f3,stroke:#333,stroke-width:2px,color:#333
    style API fill:#d9ead3,stroke:#333,stroke-width:2px,color:#333
    style Worker fill:#d9ead3,stroke:#333,stroke-width:2px,color:#333
````

### 1. Database Setup
Ensure MySQL is installed and running on your local machine. Create a new, empty database (e.g., `pulonia_db`).

### 2. `appsettings` Setup
Navigate to `src/Pulonia.API` and configure `appsettings.json` (or `appsettings.Development.json`) with your database credentials and JWT Secret.

### 3. Package Install
Run the following command to install all package needed.
```bash
dotnet restore
```

### 4. Run the Server
Use the following command to start the server. Makesure you are on `Pulonia/src/Pulonia.API` and `Pulonia/src/Pulonia.Worker` directory
```bash
dotnet run
# Your server will run on port: 5098 (or as configured in launchSettings.json)
```
### 5. Healty Check 
You can check your *server* and *database* health by accessing:
```
GET http://localhost:5098/api/health
```

### 6. Access Swagger
View the API documentation and test endpoints via Swagger at:
```text
http://localhost:5098/swagger
```

### Other CLI Commands
#### Database Migrations (Must be run from the Root Project):
```bash
# 1. Create table blueprints
dotnet ef migrations add InitialCreate --project src/Pulonia.Infrastructure --startup-project src/Pulonia.API

# 2. Execute table creation to MySQL
dotnet ef database update --project src/Pulonia.Infrastructure --startup-project src/Pulonia.API

# 3. How to update the template after pull or local changes:
dotnet new install . --force
```

## Prepush Checklist
Before committing and push your code, you are encouraged to run the verification suite locally:
```bash
# 1. Format code according to .editorconfig standards
dotnet format

# 2. Compile the application and run static analysis
dotnet build

# 3. Ensure all unit tests pass perfectly (Unit Testing not implemented yet)
dotnet test
```

## Credits & Acknowledgements

* **Hanashiro Yuriku | Dionisius Geovanni Caesario**
* **Gemini** - *AI Pair Programmer*
  Served as a technical consultant and coding assistant, providing architectural insights and collaborative debugging throughout the development journey.
* #### This project was generated using the [C&#111;lumbina Template](https://github.com/HanashiroYuriku/C&#111;lumbina).