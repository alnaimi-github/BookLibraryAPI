# BookLibraryAPI

BookLibraryAPI is a modular, layered .NET 8 Web API for managing a library of books and users, following Clean Architecture and SOLID principles. It supports JWT authentication, role-based authorization, email notifications, and is ready for containerized development with Docker and Mailpit.

## Requirements & Features

1. **Book Model**
   - Id (int), Title (string), Author (string), Year (int)
2. **Endpoints**
   - `POST /books`: Create a new book (Moderator or Admin only)
   - `GET /books`: List all books (all users)
3. **Validation**
   - All input is validated using FluentValidation.
4. **JWT Authentication & Roles**
   - JWT authentication with roles: Admin (all permissions), Moderator (create/update), User (read only)
   - Endpoints are protected by role-based authorization.
5. **Welcome Email on Book Creation**
   - When a new book is created, management is notified by email (business rule).
   - This is implemented in the Application Layer (CreateBookCommandHandler) and uses a local SMTP server (Mailpit or Papercut) for development/testing.
6. **CQRS with MediatR**
   - All commands and queries are handled via MediatR for clean separation of concerns.
7. **Error Handling Middleware**
   - Custom middleware returns errors as ProblemDetails for consistent API responses.
8. **PostgreSQL & EF Core**
   - PostgreSQL is used as the database, with EF Core for data access and migrations.
9. **Caching**
   - Book list queries are cached for performance using a cache service (`ICacheService`).
   - The cache is automatically invalidated after creating or updating a book to ensure fresh data.
   - 
     <img width="1857" height="587" alt="image" src="https://github.com/user-attachments/assets/6963661f-c350-4f73-8160-3a21834feb92" />

   - Caching is implemented in the Application layer (handlers) and the actual cache provider is implemented in the Infrastructure layer.
   - Main files:
     - `src/Application/BookLibraryAPI.Application/Common/Services/Caching/ICacheService.cs` (interface)
     - `src/Application/BookLibraryAPI.Application/Features/Books/Queries/GetAllBooks/GetAllBooksQueryHandler.cs` (cache usage)
     - `src/Application/BookLibraryAPI.Application/Features/Books/Commands/CreateBook/CreateBookCommandHandler.cs` (cache invalidation)
     - `src/Application/BookLibraryAPI.Application/Features/Books/Commands/UpdateBook/UpdateBookCommandHandler.cs` (cache invalidation)
     - `src/Infrastructure/BookLibraryAPI.Infrastructure/Services/Caching/` (implementation)

## Features
- **Books CRUD**: Create, read, update, and list books with validation and error handling.
- **User Management**: Register, login, and manage users with hashed passwords and roles (Admin, Moderator, User).
- **Authentication & Authorization**: JWT-based authentication, role-based endpoint protection.
- **Email Notifications**: Sends emails (e.g., on book creation) using MailKit and Mailpit for local development.
- **Validation**: Uses FluentValidation for DTO and command validation.
- **CQRS & MediatR**: Command and query separation with MediatR.
- **SOLID Principles**: Codebase is organized for maintainability and testability.
- **Docker-Ready**: Includes Docker Compose for API and Mailpit (SMTP testing).
- **Swagger/OpenAPI**: Interactive API documentation with JWT support.

## Project Structure
- `src/Core/BookLibraryAPI.Core.Domain`: Domain entities, value objects, interfaces, and core logic.
- `src/Application/BookLibraryAPI.Application`: Application layer with DTOs, commands, queries, validators, mappers, and MediatR handlers.
- `src/Infrastructure/BookLibraryAPI.Infrastructure`: Data access, repositories, services (email, JWT), and settings.
- `src/Presentation/BookLibraryAPI.Presentation`: API endpoints, middleware, DI, and startup configuration.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)

### Running Locally
1. **Start Mailpit (SMTP for dev):**
   ```sh
   docker compose up -d mailpit
   ```
   Mailpit Web UI: [http://localhost:8025](http://localhost:8025)

2. **Configure Email (for local dev):**
   In `appsettings.Development.json`:
   ```json
   "Email": {
     "Host": "localhost",
     "Port": "3025",
     "From": "library@example.com",
     "To": "admin@example.com"
   }
   ```
   (If running API in Docker, use `"Host": "mailpit", "Port": "1025"`)

3. **Run the API:**
   ```sh
   dotnet run --project src/Presentation/BookLibraryAPI.Presentation
   ```

4. **Swagger UI:**
   [http://localhost:7000/swagger](http://localhost:5000/swagger) (or the port in your launch settings)

## API Authentication
- Use `/api/auth/register` to create a user.
- Use `/api/auth/login` to get a JWT token.
- Use the "Authorize" button in Swagger UI to authenticate requests.

## Endpoints

### Authentication
- `POST /api/auth/register` — Register a new user
- `POST /api/auth/login` — Login and receive a JWT token

### Books
- `GET /api/books` — Get all books (requires authentication)
- `GET /api/books/{id}` — Get a book by ID (requires authentication)
- `POST /api/books` — Create a new book (requires Moderator or Admin)
- `PUT /api/books/{id}` — Update a book (requires Moderator or Admin)

## File Structure

```
BookLibraryAPI/
├── compose.yaml
├── README.md
├── src/
│   ├── Application/
│   │   └── BookLibraryAPI.Application/
│   │       ├── Common/
│   │       │   ├── DTOs/
│   │       │   │   ├── Books/
│   │       │   │   └── Users/
│   │       │   └── Mappers/
│   │       ├── Services/
│   │       │   ├── Authentication/
│   │   
│   │       ├── Features/
│   │       │   ├── Books/
│   │       │   └── Users/
│   │       └── DependencyInjection.cs
│   ├── Core/
│   │   └── BookLibraryAPI.Core.Domain/
│   │       ├── Books/
│   │       ├── Users/
│   │       │   └── Enums/
│   │       ├── Common/
│   │       │   └── Exceptions/
│   │       ├── Interfaces/
│   │       │   ├── Ports/
│   │       │   └── Repositories/
│   │       └── ValueObjects/
│   ├── Infrastructure/
│   │   └── BookLibraryAPI.Infrastructure/
│   │       ├── Adapters/
│   │       │   └── Email/
                └── Caching/
│   │       ├── Persistence/
│   │       │   ├── Configurations/
│   │       │   └── Migrations/
│   │       ├── Repositories/
│   │       ├── Services/
│   │       │   ├── Authentication/
│   │      
│   │       └── Settings/
│   │       └── DependencyInjection.cs
│   └── Presentation/
│       └── BookLibraryAPI.Presentation/
│           ├── Endpoints/
│           ├── Extensions/
│           ├── Middleware/
│           ├── appsettings.Development.json
│           ├── appsettings.json
│           ├── Program.cs
│           └── Dockerfile
```

## Docker Compose Example
```yaml
services:
  mailpit:
    image: axllent/mailpit:latest
    ports:
      - "8025:8025"
      - "3025:1025"
    restart: unless-stopped
```

## Business Rules

- When a new book is created, management must be notified by email.
  - This is a business requirement, not just a technical improvement.
  - The email notification logic is implemented in the Application Layer (specifically, in the CreateBookCommandHandler) and is triggered after a book is successfully added.
  - This ensures the business rule is enforced as part of the use case logic.

<img width="1858" height="790" alt="image" src="https://github.com/user-attachments/assets/20484ea9-27ec-4f7b-a3f5-1abb74578b16" />


## Architecture

This project follows the principles of **Hexagonal Architecture (Ports and Adapters)**:

- **Core Domain**: Contains business entities, value objects, and domain logic. It is independent of any external frameworks or technologies.
- **Application Layer**: Contains use cases (commands, queries, handlers) and orchestrates business rules. It depends only on the core domain and defines ports (interfaces) for external interactions.
- **Ports**: Interfaces in the Core or Application layer that define contracts for external services (e.g., repositories, email notifications).
- **Adapters**: Infrastructure implementations of ports (e.g., EF Core repositories, email services). These are injected into the application via dependency injection.
- **Presentation Layer**: Minimal API endpoints, middleware, and configuration. This layer is responsible for HTTP, validation, and error handling, and delegates business logic to the Application layer.

This separation ensures:
- Business logic is isolated and testable.
- Infrastructure and frameworks can be swapped with minimal impact.
- The system is maintainable, extensible, and follows Clean Architecture best practices.

  <img width="1024" height="1024" alt="ChatGPT Image 5 أغسطس 2025، 02_22_53 ص" src="https://github.com/user-attachments/assets/db05c759-64ed-4cd7-aa4f-97a4e697a9f8" />


## Extending the System
- Add new features in the Application layer (commands, queries, handlers).
- Add new endpoints in the Presentation layer.
- Add new services (e.g., notifications) in the Infrastructure layer.

## Contributing
Pull requests are welcome! Please follow the existing code style and add tests for new features.

## License
MIT

## Recent Changes & Improvements

### Docker & HTTPS
- The API now supports HTTPS in Docker using a development certificate.
- Default HTTPS port is `9443` (see `compose.yaml` and `appsettings.json`).
- Access the API in Docker at: `https://localhost:9443/swagger`

### Email Notification Reliability
- The `Email` section in `appsettings.json` now includes a required `To` field to avoid null recipient errors.
- SMTP configuration keys in code were fixed to match the settings (`SmtpHost`, `SmtpPort`).

### CI/CD Pipeline
- The GitHub Actions workflow (`.github/workflows/ci-cd.yml`) now includes:
  - Build, test (unit & integration), and artifact upload.
  - Docker image build and push to DockerHub.
  - Docker Compose deployment.
  - Mailpit service for email integration tests, ensuring all tests pass in CI.

## How to Check if the Application is Running

### Locally (Docker)

<img width="1546" height="348" alt="image" src="https://github.com/user-attachments/assets/e2e4b0e7-d022-4086-9942-535eb47569ac" />

<img width="1466" height="326" alt="image" src="https://github.com/user-attachments/assets/a330d555-b852-4fef-a88e-f02266e83246" />
<img width="1800" height="865" alt="image" src="https://github.com/user-attachments/assets/9ecae788-dcb9-4698-b9c6-28330866e221" />


1. Run:
   ```sh
   docker compose up -d or docker pull abdulwaisa/booklibraryapi
   ```
2. Visit:
   - API: `https://localhost:9443/swagger`
   - Mailpit UI: `http://localhost:8025`
3. Check logs:
   ```sh
   docker compose logs app
   ```

### In CI/CD (GitHub Actions)
- On every push/PR to `main`, the workflow will:
  - Build and test the app.
  - Deploy the latest image with Docker Compose.
  - You can view workflow status and logs in the GitHub Actions tab.

## Testing

<img width="971" height="860" alt="image" src="https://github.com/user-attachments/assets/70dcf8fb-92e9-44fd-bd00-9a62cee1fe59" />

The project includes comprehensive unit and integration tests:

- **Unit Tests:** Located in `tests/BookLibraryAPI.UnitTests`. Run with:
  ```sh
  dotnet test tests/BookLibraryAPI.UnitTests/BookLibraryAPI.UnitTests.csproj
  ```
- **Integration Tests:** Located in `tests/BookLibraryAPI.IntegrationTests`. These use Testcontainers for PostgreSQL, Redis, and Mailpit. Run with:
  ```sh
  dotnet test tests/BookLibraryAPI.IntegrationTests/BookLibraryAPI.IntegrationTests.csproj
  ```
- **CI/CD:** All tests are automatically run in GitHub Actions on every push and pull request to `main`. Test results are uploaded as artifacts.

You can view test results in the GitHub Actions tab or by running the above commands locally.

## 🧪 Test User Accounts

You can use the following predefined users to log in during development/testing:

| Role       | Username   | Password   |
|------------|------------|------------|
| Admin      | `admin`    | `admin123` |
| Moderator  | `moderator`| `mod123`   |
| User       | `user`     | `user123`  |

🔐 **Note:** Passwords are hashed using BCrypt at runtime. The plain-text versions above are only for testing purposes.

