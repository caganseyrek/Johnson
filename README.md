# Johnson

Johnson is a discovery and proxy service designed for managing server groups, built as part of a personal learning project.

> This project is **NOT** intended for production use (at least currently). It was developed primarily as a learning exercise to explore concepts related to server orchestration, discovery, and routing. If you intend to use this in a production environment, please make sure to do proper testing and validation before.

## Features

- **Service Discovery**: Register and track the availability of servers within a group.
- **Proxying**: Route external API requests to the appropriate services.
- **Fault Tolerance**: Provides basic fault tolerance mechanisms for service availability.

## Technologies

- **ASP.NET Core Web API** - Main framework used in the project.
- **Entity Framework Core** - Used for database access.
- **RabbitMQ** - Used messaging and communication between modules.
- **Redis** - Used caching server details for faster data retrievals.
- **SQLite** - Acts as a backup and permanent storage for cached data, ensuring persistence in case of system failure.
- **RabbitMQ** - For environment setup (RabbitMQ and Redis).

## Installation and Setup

This project requires Docker to set up RabbitMQ and Redis for local development. Here are the steps to get it running:

### Prerequisites

- Docker
- .NET 8 or higher
- SQLite3

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/caganseyrek/Johnson.git
   ```

2. Navigate to the project directory:

   ```bash
   cd path/to/Johnson
   ```

3. Run docker container for RabbitMQ and Redis using Docker Compose:

   ```bash
   docker-compose up
   ```

4. Build the ASP.NET Core Web API project:

   ```bash
   dotnet build
   ```

5. Apply migrations to the SQLite database:

   ```bash
   dotnet ef database update
   ```

6. Apply migrations to the SQLite database:

   ```bash
   dotnet run
   ```

### Stopping the Services

To stop the Docker services (RabbitMQ and Redis), run:

```bash
docker-compose down
```

## License

This project is open-source and licensed under [MIT License](https://github.com/caganseyrek/Johnson/blob/main/LICENSE).
