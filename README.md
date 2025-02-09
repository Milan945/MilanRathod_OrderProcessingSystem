# Order Processing System

## Overview
The **Order Processing System** is a simplified e-commerce order management API built using **.NET 8** and **Entity Framework Core**. This system enables the management of customers, products, and orders, ensuring validation rules and business logic for order processing.

## Features
- **Customer Management**: Create and retrieve customer information.
- **Order Processing**: Place new orders, retrieve order details, and enforce validation rules.
- **Product Catalog**: Manage products and calculate order totals based on product prices.
- **Validation Rules**: Prevent orders if a customer has an unfulfilled order.
- **Order Fulfillment**: Allows fulfilling orders once processed.
- **User Authentication**: Supports user registration and login.
- **Logging & Error Handling**: Uses Serilog for structured logging.
- **Unit Testing**: Implemented using **Microsoft.Testing** and **Moq** for mocking dependencies.
- **Async Operations**: All database interactions are handled asynchronously.
- **Containerization**: Docker support for easy deployment.

## Technologies Used
- **.NET 8**
- **Entity Framework Core**
- **ASP.NET Core Web API**
- **Microsoft.Testing & Moq** (for Unit Testing)
- **Serilog** (for Logging)
- **Docker** (for Containerization)
- **SQL Server** (for Database)

## Getting Started
### Prerequisites
- .NET 8 SDK ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- SQL Server (Local or Dockerized instance)
- Docker (optional, for containerization)

### Setup Instructions
#### 1. Clone the Repository
```sh
git clone https://github.com/Milan945/MilanRathod_OrderProcessingSystem.git
cd MilanRathod_OrderProcessingSystem
```
#### 2. Configure Database
- Update **appsettings.json** with the appropriate SQL Server connection string:
```json
"ConnectionStrings": {
  "OrsDatabase": "Server=your_server;Database=OrderDB;User Id=your_user;Password=your_password;"
}
```
- Run database migrations:
```sh
dotnet ef database update
```
#### 3. Run the Application
```sh
dotnet run
```

## API Endpoints
### Customer Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET | `/api/customers` | Retrieve all customers |
| GET | `/api/customers/{id}` | Retrieve a specific customer and their orders |

### Order Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST | `/api/orders` | Create a new order |
| GET | `/api/orders/{id}` | Retrieve order details (including total price) |
| POST | `/api/orders/{id}/fulfill` | Fulfill an existing order |

### User Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST | `/api/users/register` | Register a new user |
| POST | `/api/users/login` | Authenticate a user and obtain a token |

## Business Rules
- An order cannot be placed if the customer's previous order is unfulfilled.
- Order total is calculated based on the product prices.
- Users must register and authenticate before placing an order.

## Running Tests
Run unit tests using:
```sh
dotnet test
```

## Docker Support
### Build & Run in Docker
```sh
docker build -t order-processing-system .
docker run -p 5000:5000 order-processing-system
```

## Assumptions & Design Decisions
- Products are predefined and cannot be added dynamically.
- Orders are immutable once placed.
- Unfulfilled orders are determined based on a boolean `IsFulfilled` field in the `Orders` table.
- Users must be registered and logged in to place orders.

## Contributing
Feel free to fork this repository and submit pull requests for enhancements or bug fixes.

## License
This project is open-source and licensed under the MIT License.

---
### GitHub Repository: [MilanRathod_OrderProcessingSystem](https://github.com/Milan945/MilanRathod_OrderProcessingSystem)

