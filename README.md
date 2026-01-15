# Service Marketplace API

This project is a comprehensive RESTful Web API application developed as a final project for the **Backend Development** course. It serves as a marketplace infrastructure connecting service providers (cleaners, painters, movers, etc.) with customers seeking these services.

## 📋 Project Description and Purpose
Service Marketplace API offers a platform where users can create service listings, book appointments for these services, and rate/review the services they receive. The primary goal of the project is to build an architecture that is scalable, secure, and adheres to modern backend principles (SOLID, Clean Architecture).

## 🛠 Technologies Used

The following technologies and libraries were used in this project:

* **Framework:** ASP.NET Core Web API (.NET 10.0)
* **Data Access:** Entity Framework Core (Code First Approach)
* **Database:** PostgreSQL
* **Authentication:** JWT (JSON Web Token) & ASP.NET Core Identity
* **Real-Time Communication:** SignalR (For notifications)
* **Caching:** In-Memory Caching (For service category listing performance)
* **Containerization:** Docker & Docker Compose
* **API Documentation:** Swagger (Swashbuckle)

## 🚀 Installation and Execution Instructions

Follow the steps below to run the project in your local environment.

### Prerequisites
* .NET SDK (Version 10.0 or compatible)
* Docker Desktop (To run with Docker)
* PostgreSQL (If running locally without Docker)

### Method 1: Running with Docker (Recommended)
The project supports Docker Compose to spin up the API and Database with a single command.

1. Clone the repository:
   
   git clone [https://github.com/username/repo-name.git](https://github.com/username/repo-name.git)
   cd service-marketplace-api 

2. Create a .env file in the root directory and define the required variables (or use the defaults in docker-compose.yml):

DB_PASSWORD=SuperSecretPassword123!
PGADMIN_EMAIL=admin@admin.com
PGADMIN_PASSWORD=admin
JWT_KEY=ThisIsASecretKey_MustBeAtLeast32CharsLong

3. Start the application

docker-compose up --build

4. Navigate to the following address in your browser: http://localhost:8080/swagger


### Method 2: Running Locally (Localhost)

1. Update the database connection string in appsettings.json according to your local PostgreSQL server.

2. Open a terminal and apply database migrations:

dotnet ef database update

3. Start the project:

dotnet run


### API Documentation
When the project is running, you can access the Swagger interface to test all API endpoints at the following address:

Swagger UI: http://localhost:5110/swagger (Local) or http://localhost:8080/swagger (Docker)

Key Features and Endpoints

Auth: Register (/auth/register), Login (/auth/login)

Services: Create, Update, Delete Listings and Advanced Filtering (City, Price, Category, etc.) (/api/services)

Appointments: Request, Approve/Reject, and Complete Appointments (/api/appointments)

Reviews: Rate and review completed services (/api/review)

Notifications: Live notifications via SignalR (/notificationHub)

### Team Members
Name Surname: Ardil SUNGU - 1031510019

Name Surname: Enes TAS - 1031510011

Name Surname: Burakcan GIDIS - 1031510033

### Project Management
[Trello/Jira/GitHub Projects] was used for task distribution and tracking during the development process.

Project Board Link: https://trello.com/b/KVLECgXo/servicemarketplace

### Live Deployment
The project has been deployed on [Azure / Render / Railway].

Live API URL: https://isedin.onrender.com/swagger/index.html



### Architecture and Design Decisions
N-Layer Architecture: The project is designed following SOLID principles, separated into Controller, Service, Data Access (Repository/DbContext), and Model layers.

DTO Pattern: DTOs (Data Transfer Objects) are used instead of Entity models for API data transfer to enhance security and performance.

Global Exception Handling: Middleware has been implemented to catch errors centrally.
