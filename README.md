# Coffee Machine API

This is a simple .NET Core API project that simulates a coffee machine. It allows you to brew coffee, with specific behaviors on April 1st and a limit on the number of requests allowed before the service becomes unavailable. It uses Entity Framework Core with an Azure SQL database, but during testing, it can switch to an in-memory database.

## Features

- **Brew Coffee**: Allows users to brew coffee.
- **Service Limit**: After every 5th request, the service becomes unavailable and returns a `503 Service Unavailable` response.
- **April 1st Prank**: On April 1st, the API returns a `418 I'm a teapot` response as a prank.
- **Database Integration**: Data is stored in a database (Azure SQL or SQLite in-memory for testing).
- **Unit & Integration Tests**: The project includes unit and integration tests to ensure functionality works as expected.

## Getting Started

To get the project up and running on your local machine, follow these steps:

### Prerequisites

- .NET 6 or later
- Visual Studio (or VS Code)
- SQL Server (Azure SQL for production) or SQLite (in-memory database for testing)
- Git (optional, for version control)
- Postman or any HTTP client (to test the API)

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/yourusername/your-repository-name.git
   cd your-repository-name
