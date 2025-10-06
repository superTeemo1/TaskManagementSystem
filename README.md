# 📝 Task Management System – ASP.NET Core (.NET 8)

A simple REST API and MVC web application for managing tasks, built with **ASP.NET Core 8**, **C#**, and **Razor Views**.  
The system uses an **in-memory repository** (no database) and supports basic CRUD operations.

---

## 🚀 Features

✅ Create, read, update, and delete tasks  
✅ Search and filter by title, description, or status  
✅ REST API endpoints (`/api/tasks`)  
✅ In-memory storage (no external DB)  
✅ MVC frontend with Razor Views  
✅ Simple and clean UI  
✅ Ready for testing and extension

---

## 🧩 Technologies & Architecture

**Framework:** .NET 8  
**Frontend:** ASP.NET Core MVC (Razor Views)  
**Backend:** C# – layered architecture (Service + Repository)  
**Dependency Injection:** Built-in  
**Persistence:** InMemoryTaskRepository  

**Solution structure:**
```
TaskManagement/
 ├─ TaskManagement.Domain          # Models and interfaces
 ├─ TaskManagement.Application     # DTOs and business logic
 ├─ TaskManagement.Infrastructure  # In-memory repository
 ├─ TaskManagement.Web             # MVC UI and REST API
 └─ TaskManagement.Tests           # Unit tests (optional)
```

---

## ⚙️ How to Run Locally

### 1️⃣ Requirements
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) (v17.8 or later)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### 2️⃣ Clone the repository
```bash
git clone https://github.com/superTeemo1/TaskManagementSystem
cd TaskManagement
```

### 3️⃣ Run the application
Using Visual Studio:
- Set **TaskManagement.Web** as *Startup Project*
- Press **F5** or **Ctrl+F5**

Using CLI:
```bash
cd TaskManagement.Web
dotnet run
```

### 4️⃣ Open in your browser
```
https://localhost:5001/Tasks
```

---

## 🔗 API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/tasks` | Retrieve all tasks |
| `GET` | `/api/tasks/{id}` | Retrieve a specific task |
| `POST` | `/api/tasks` | Create a new task |
| `PUT` | `/api/tasks/{id}` | Update an existing task |
| `DELETE` | `/api/tasks/{id}` | Delete a task |

### 📘 Example `POST` request
```json
POST /api/tasks
{
  "title": "Implement frontend",
  "description": "Create Razor views for task management",
  "status": "InProgress"
}
```

---

## 🌐 Deployment (optional)

To deploy online:
- Use **Azure App Service**, **Render**, or any .NET-capable host.
- Build release version:
  ```bash
  dotnet publish -c Release -o ./publish
  ```
  Deploy the contents of the `publish` folder.

---

## 🧠 Testing (optional)

Run tests:
```bash
cd TaskManagement.Tests
dotnet test
```

---

## 👨‍💻 Author & Notes

This project is part of a **.NET Developer technical assessment**.  
It demonstrates clean architecture, separation of concerns, and production-level code quality using an in-memory persistence layer.

---

## 📄 License
MIT License (optional)
