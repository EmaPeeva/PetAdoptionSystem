


# PetAdoptionSystem

PetAdoptionSystem is a full-stack web application for managing pet adoptions. It uses an ASP.NET Core Web API backend (.NET 8.0) with a frontend built in HTML, CSS, and JavaScript served from the backend’s `wwwroot` folder.

---

## Table of Contents

- [Project Overview](#project-overview)  
- [Features](#features)  
- [Technology Stack](#technology-stack)  
- [Project Structure](#project-structure)  
- [Prerequisites](#prerequisites)  
- [Setup and Run](#setup-and-run)  
- [Usage](#usage)  
- [Contributing](#contributing)  
- [License](#license)  
- [Contact](#contact)  

---

## Project Overview

Users can register, log in, view available pets, and submit adoption requests. Admin users manage pets and adoption requests, approving or rejecting them, with email notifications triggered upon approval.

---

## Features

- User registration and login  
- Browse and search pets  
- Submit and track adoption requests  
- Admin dashboard for pet and request management  
- Email notifications for adoption approvals  
- User profile management  
- Role-based access control  

---

## Technology Stack

- Backend: ASP.NET Core Web API 8.0 (.NET 8.0)  
- Frontend: HTML, CSS, JavaScript (served from `wwwroot`)  
- Database: PostgreSQL  
- ORM: Entity Framework Core  
- Testing: xUnit  

---

## Project Structure

```

PetAdoptionSystem/
│
├── Controllers/          # API controllers
├── Data/                 # Database context and migrations
├── DTOs/                 # Data transfer objects
├── Migrations/           # EF Core migration files
├── Models/               # Entity models
├── Repositories/         # Data access layer
├── Services/             # Business logic services
├── wwwroot/              # Frontend static files
├── Properties/           # Project properties
├── appsettings.json      # Configuration
├── Program.cs            # Application entry point
├── PetAdoptionSystem.sln # Solution file
└── PetAdoptionSystem.Tests/ # Unit tests

````

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [PostgreSQL](https://www.postgresql.org/download/)  

> **Important:** Update your database connection string in `appsettings.json` with your own credentials. 

Example connection string format:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=PetAdoptionDB;Username=your_username;Password=your_password"
}
````

---

## Setup and Run

1. **Clone the repository:**

```bash
git clone https://github.com/EmaPeeva/PetAdoptionSystem.git
cd PetAdoptionSystem
```

2. **Restore dependencies and build the project:**

```bash
dotnet restore
dotnet build
```

3. **Apply database migrations:**

```bash
dotnet ef database update
```

4. **Run the application:**

```bash
dotnet run
```



## Usage

* **Users:** Register, log in, browse pets, submit adoption requests, and manage profiles.
* **Admins:** Manage pets and adoption requests, approve/reject requests, and trigger notification emails.

---
## 📸 Screenshots

### 🔑 Register Page
![Register](docs/screenshots/register.png)

### 🔐 Login Page
![Login](docs/screenshots/login.png)

### 🐾 User - Main View (Pet List)
![User Main View](docs/screenshots/uses-main-view.png)

### 📋 User - Adoption Requests
![User Adoption Requests](docs/screenshots/users-adopt-requests.png)

### 🙍‍♀️ User - Profile
![User Profile](docs/screenshots/users-profile.png)

---

### 🛠️ Admin - Main Dashboard
![Admin Main](docs/screenshots/admin-main-view.png)

### 📑 Admin - All Adoption Requests
![Admin Adoption Requests](docs/screenshots/admin-alladopt-requests.png)

### 📄 Admin - Add New Pet
![Admin Add Pet](docs/screenshots/admin-addpet.png)

### ✏️ Admin - Edit Pet
![Admin Edit Pet](docs/screenshots/admin-edit-pet.png)

### 👥 Admin - All Adopters
![All Adopters](docs/screenshots/all-adopters-list.png)

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---

## License

 MIT License

---

## Contact

For questions or suggestions, please open an issue or contact via GitHub profile:
[https://github.com/EmaPeeva](https://github.com/EmaPeeva) 

---

*Thank you for visiting the PetAdoptionSystem repository!*

```



