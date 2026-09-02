# Login System – Developer Assessment

## Overview

This project is a full-stack authentication system built as part of a developer assessment. It demonstrates how a typical login workflow works in a modern web application using a clean and maintainable architecture.

The application allows users to: 

- Register a new account
- Log in using their credentials
- Retrieve their user details after authentication
- 
![image alt](https://github.com/Msekeli/LoginSystem/blob/103a00777042f99d43536439482e75a226bc67cd/auth-system-screenshots.png)

The backend exposes REST API endpoints which are consumed by a React frontend application.

## Tech Stack

### Backend

- .NET 9 Web API
- Clean Architecture
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- BCrypt Password Hashing

### Frontend

- React
- Vite
- Tailwind CSS
- JavaScript

### Infrastructure

- Docker
- Docker Compose

---

## Project Structure

LoginSystem  
│  
├── src  
│ ├── AuthService.Api  
│ ├── AuthService.Application  
│ ├── AuthService.Domain  
│ └── AuthService.Infrastructure  
│  
├── auth-frontend  
│  
├── docker  
│  
└── tests

Architecture details are explained in **ARCHITECTURE.md**.

---

## API Endpoints

### Register User

POST /api/auth/register

Example request:

{
"firstName": "John",
"lastName": "Doe",
"email": "john@example.com",
"password": "Password123"
}

---

### Login User

POST /api/auth/login

Example request:

{
"email": "john@example.com",
"password": "Password123"
}

Example response:

{
"token": "JWT_TOKEN"
}

---

### Get Authenticated User

GET /api/auth/me

Requires Authorization header:

Authorization: Bearer <token>

Returns the authenticated user's details.

---

## Running the Backend

### Requirements

- .NET 9 SDK
- PostgreSQL

Create database:

createdb authdb

Run migrations:

dotnet ef migrations add InitialCreate --project src/AuthService.Infrastructure --startup-project src/AuthService.Api

dotnet ef database update --project src/AuthService.Infrastructure --startup-project src/AuthService.Api

Run the API:

dotnet run --project src/AuthService.Api

Swagger will be available at:

http://localhost:8080/swagger

---

## Running the Frontend

Navigate to the frontend folder:

cd auth-frontend

Install dependencies:

npm install

Run development server:

npm run dev

Frontend will be available at:

http://localhost:5173

---

## Running with Docker

The application can also be started using Docker:

docker compose up --build

This will start:

- API container
- PostgreSQL container

---

## Testing the Application

1. Register a new user
2. Log in with the created account
3. Copy the JWT token returned
4. Use the token to call the protected `/api/auth/me` endpoint

---

## Notes

The project follows Clean Architecture principles to ensure clear separation between business logic, application logic, and infrastructure concerns.
