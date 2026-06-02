# ✈️ TravelAway-WebApp

A modern full-stack travel booking web application built using **Angular 18**, **.NET 8 Web API**, and **SQL Server**.

The platform provides a luxury travel booking experience with authentication, package browsing, booking workflow, payment simulation, and responsive UI design.

---

# 🌍 Project Overview

TravelAway is designed using a scalable full-stack architecture with a modular frontend and clean backend separation.

The application includes:

* JWT Authentication
* Travel Package Browsing
* Smart Search & Filtering
* Booking Workflow
* Dummy Payment Integration
* Responsive UI
* Lazy Loading
* REST APIs
* SQL Server Integration

---

# 🛠 Tech Stack

## 🎨 Frontend

* Angular 18
* TypeScript
* SCSS
* RxJS
* Angular Signals
* Standalone Components

---

## ⚙ Backend

* .NET 8 Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Repository Pattern
* Clean Architecture

---

# 📂 Project Structure

```text
TravelAway-WebApp
 ┣ TravelAway-Backend
 ┃ ┣ Voyara.API
 ┃ ┣ Voyara.Core
 ┃ ┣ Voyara.Infrastructure
 ┃ ┗ Voyara.Shared
 ┃
 ┣ TravelAway-Frontend
 ┃ ┗ src/app
 ┃    ┣ components
 ┃    ┣ core
 ┃    ┣ feature
 ┃    ┗ shared
 ┃
 ┗ README.md
```

---

# ✨ Features

## 🔐 Authentication

* JWT-based authentication
* Login system
* Session persistence
* Auth interceptor

---

## 🏖 Travel Packages

* Dynamic package listing
* Filtering & sorting
* Responsive package cards
* Search functionality

---

## 🧾 Booking Workflow

Multi-step booking process:

1. Travelers
2. Personal Information
3. Travel Dates
4. Payment

---

## 💳 Payment Module

* Dummy Razorpay-style payment flow
* Booking creation before payment
* Simulated payment success
* Payment persistence in database

> Note: This project uses dummy/test payment implementation only.

---

# ⚡ Performance Optimizations

* Angular Lazy Loading
* Async Pipes
* Angular `@for`
* Standalone Components
* Optimized API Calls
* Reduced unnecessary re-renders

---

# 🚀 Frontend Setup

```bash
cd TravelAway-Frontend
npm install
ng serve
```

Frontend runs on:

```text
http://localhost:4200
```

---

# 🚀 Backend Setup

```bash
cd TravelAway-Backend
dotnet restore
dotnet ef database update
dotnet run
```

Backend runs on:

```text
http://localhost:5237
```

Swagger:

```text
http://localhost:5237/swagger
```

---

# 🗄 Database

* SQL Server
* Entity Framework Core Migrations included

# Database Setup

This project uses SQL Server with Entity Framework Core.

## Restore Database Using EF Core

Run migrations:

```bash
dotnet ef database update
```
---

## Alternative Manual Setup

Execute:

```text
schema.sql
```

inside SQL Server Management Studio.

---

## Notes

* Only schema/sample data is included
* No production credentials or sensitive data are stored


---

# 🔐 Security Note

This repository does not contain:

* Real payment gateway credentials
* Production JWT secrets
* Production database credentials

All sensitive configuration values are stored locally.

---

# 🧠 Development Note

This project was primarily developed with the assistance of AI tools (mainly Claude) for architecture, frontend, and backend implementation, while customization, debugging, integration fixes, optimization, and project structuring were handled manually.

---

# 📈 Future Improvements

* Real Payment Gateway Integration
* Admin Dashboard
* Booking History
* Email Notifications
* Docker Support
* CI/CD Pipeline
* Unit Testing & Integration Testing

---

# 👨‍💻 Author

Pranav Rawat

Full Stack Developer
Tech Stack:

* .NET
* Angular
* SQL Server
* REST APIs

---

# ⭐ Support

If you like this project, consider giving it a ⭐ on GitHub.
