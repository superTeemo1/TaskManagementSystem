# Task Management System

A simple web application for managing tasks.  
Users can create, edit, view, delete, and filter tasks by title or status.

The project was built as a practical example of applying clean architecture principles using ASP.NET Core.

---

## Features

- Create, edit, and delete tasks  
- Search and filter tasks by title or status  
- Validation of all user inputs  
- Display of error and success messages  
- Simple and responsive interface  
- File-based logging and exception handling  
- Unit tests for key application layers  

---

## Architecture Overview

This project follows a **clean layered architecture**:

| Layer | Description |
|--------|-------------|
| **Domain** | Core entities and business logic |
| **Application** | DTOs, validation, and service layer |
| **Infrastructure** | Repository implementations and data access |
| **Web** | ASP.NET Core MVC layer (controllers, Razor views, error handling) |
| **Tests** | Unit tests for services and repositories using xUnit |

---

## Technologies Used

- **.NET 8 / ASP.NET Core MVC**  
- **C#**  
- **Razor Views**  
- **Dependency Injection**  
- **xUnit** for testing  
- **File-based logging (ILogger)**  
- **Docker** for containerization  

---

## Getting Started (Run Locally)

1. Clone the repository:
   ```bash
   git clone https://github.com/superTeemo1/TaskManagementSystem.git
   ```

2. Navigate to the web project:
   ```bash
   cd TaskManagement/TaskManagement.Web
   ```

3. Restore dependencies and run the application:
   ```bash
   dotnet restore
   dotnet run
   ```

4. Open the app in your browser:
   ```
   https://localhost:7278
   ```

---

## Run with Docker

You can also run the application in a Docker container:

```bash
docker build -t taskmanagement .
docker run -p 8080:8080 taskmanagement
```

The app will be available at:  
👉 http://localhost:8080

---

## Running Tests

To run all unit tests:

```bash
cd TaskManagement.Tests
dotnet test
```

Tests cover:
- Validation logic (ServiceValidationTests)
- Repository behavior (RepositoryInvariantTests)
- Edge cases (invalid inputs, missing entities, etc.)

---

## Deployment

The project is deployed using **Render**.  
You can view the live version here:  
👉 [https://taskmanagementsystem-u1qr.onrender.com/](https://taskmanagementsystem-u1qr.onrender.com/)

Deployment uses a Docker-based setup to ensure consistent environment configuration.

---

## Project Goals

This project demonstrates:
- Application of clean architecture and separation of concerns  
- Input validation and structured error handling  
- Layered testing approach  
- MVC pattern with Razor views  
- Logging and observability via file logs  
- Deployment of .NET applications using Docker and Render  

---

## Author

**Jasmin**  
Built as part of a .NET clean architecture and testing exercise.
