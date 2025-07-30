# CoreWebApp - ASP.NET Core Web Application

A modern ASP.NET Core 8.0 web application built with Razor Pages, featuring user authentication, database management, and dynamic navigation.

## 🚀 Technology Stack

### **Core Framework**

- **ASP.NET Core 8.0** - Modern web framework
- **Razor Pages** - Page-based programming model
- **C# 11** - Programming language

### **Database & ORM**

- **SQLite** - Lightweight, serverless database
- **Entity Framework Core 8.0** - Object-Relational Mapping (ORM)
- **Code-First Approach** - Database schema defined in C# models

### **Frontend & UI**

- **Bootstrap 5** - Responsive CSS framework
- **Bootstrap Icons** - Icon library
- **jQuery** - JavaScript library
- **Custom CSS** - Enhanced styling

### **Authentication & Session**

- **Custom Session-Based Authentication** - User session management
- **Dependency Injection** - Service management
- **Password Hashing** - Secure password storage

## 🏗️ Architecture Overview

### **Project Structure**

```
CoreWebApp/
├── Pages/                 # Razor Pages
│   ├── Shared/           # Layout and partial views
│   ├── Login.cshtml      # Authentication page
│   ├── Companies.cshtml  # Company management
│   └── ...
├── Models/               # Data models
│   ├── User.cs          # User entity
│   ├── Company.cs       # Company entity
│   ├── Chatbot.cs       # Chatbot entity
│   └── LLM.cs          # LLM entity
├── Services/            # Business logic layer
│   ├── IUserService.cs  # User service interface
│   ├── UserService.cs   # User service implementation
│   ├── IAuthService.cs  # Authentication service interface
│   ├── AuthService.cs   # Authentication service implementation
│   └── ...
├── Data/               # Data access layer
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   ├── IDbInitializer.cs       # Database initializer interface
│   └── DbInitializer.cs        # Database initializer implementation
├── wwwroot/           # Static files
│   ├── css/          # Stylesheets
│   ├── js/           # JavaScript files
│   └── lib/          # Library files
└── files/            # Application files
    └── spec_import.csv  # CSV data for import
```

## 🔧 Key Features

### **1. User Authentication System**

- **Session-based authentication** with secure password hashing
- **Login/Logout functionality** with dynamic navigation
- **User management** with CRUD operations
- **Session persistence** across requests

### **2. Database Management**

- **SQLite database** with automatic creation
- **Entity Framework Core** for data access
- **Code-first migrations** for schema management
- **Data seeding** with default users

### **3. Dynamic Navigation**

- **Conditional menu items** based on authentication status
- **Responsive design** with Bootstrap 5
- **Clean UI** with Bootstrap Icons

### **4. Company Management**

- **Company listing** with detailed information
- **Related data** (Chatbots, LLMs) display
- **Modal dialogs** for detailed views
- **Authentication required** access

### **5. Data Import System**

- **CSV import functionality** for bulk data
- **Error handling** and validation
- **Progress logging** and status reporting

## 🛠️ Implementation Details

### **Service Layer Architecture**

```csharp
// Service interfaces for dependency injection
public interface IUserService
public interface IAuthService
public interface ICsvImportService
public interface IDbInitializer
```

### **Database Models**

```csharp
// Entity models with relationships
public class User
public class Company
public class Chatbot
public class LLM
```

### **Authentication Flow**

1. **Login Page** → User enters credentials
2. **AuthService** → Validates credentials against database
3. **Session Storage** → Stores user ID in session
4. **Navigation Update** → Shows/hides menu items based on auth status

### **Database Relationships**

- **Company** → **Chatbots** (One-to-Many)
- **Company** → **LLMs** (One-to-Many)
- **User** → **Authentication** (Independent)

## 🚀 Getting Started

### **Prerequisites**

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQLite (included with EF Core)

### **Installation**

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd CoreWebApp
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Run the application**

   ```bash
   dotnet run
   ```

4. **Access the application**
   - Open browser to `https://localhost:5001` or `http://localhost:5000`
   - Default credentials: `admin@example.com` / `password123`

### **Database Setup**

The application automatically:

- Creates SQLite database file (`CoreWebApp.db`)
- Seeds initial user data
- Creates all required tables

### **Data Import**

1. Navigate to **Import Data** page (when logged in)
2. Click **"Import CSV Data"** button
3. View imported companies on **Companies** page

## 🔐 Authentication

### **Default Users**

- **Admin:** `admin@example.com` / `password123`
- **User:** `user@example.com` / `password123`

### **Session Management**

- **Session timeout:** 30 minutes
- **Secure cookies:** HttpOnly and Essential flags
- **Session storage:** In-memory cache

## 📊 Database Schema

### **Users Table**

```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    LastLoginAt TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1
);
```

### **Companies Table**

```sql
CREATE TABLE Companies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);
```

### **Chatbots Table**

```sql
CREATE TABLE Chatbots (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    CompanyId INTEGER NOT NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE
);
```

### **LLMs Table**

```sql
CREATE TABLE LLMs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Specialization TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    CompanyId INTEGER NOT NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE
);
```

## 🎨 UI/UX Features

### **Responsive Design**

- **Mobile-first** approach with Bootstrap 5
- **Flexible layouts** that adapt to screen size
- **Touch-friendly** navigation and buttons

### **Modern Interface**

- **Clean, minimalist design** with Bootstrap components
- **Consistent styling** across all pages
- **Professional appearance** with proper spacing and typography

### **User Experience**

- **Intuitive navigation** with clear labels
- **Loading states** and error handling
- **Modal dialogs** for detailed information
- **Form validation** with helpful error messages

## 🔧 Configuration

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=CoreWebApp.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### **Program.cs Services**

```csharp
// Core services
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddDbContext<ApplicationDbContext>();

// Custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
```

## 🧪 Testing

### **Manual Testing**

1. **Authentication Flow**

   - Test login with valid/invalid credentials
   - Verify session persistence
   - Test logout functionality

2. **Navigation**

   - Verify menu items show/hide based on auth status
   - Test responsive navigation on mobile

3. **Data Operations**
   - Import CSV data
   - View companies and related data
   - Test database operations

### **Database Testing**

- Verify table creation
- Check data relationships
- Test data import functionality

## 📝 Development Notes

### **Best Practices Implemented**

- **Dependency Injection** for loose coupling
- **Interface-based design** for testability
- **Async/await** for non-blocking operations
- **Proper error handling** and logging
- **Security considerations** (password hashing, session management)

### **Future Enhancements**

- **User registration** functionality
- **Password reset** capabilities
- **Role-based authorization**
- **API endpoints** for external integration
- **Unit tests** and integration tests
- **Docker containerization**

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Built with ❤️ using ASP.NET Core 8.0**
