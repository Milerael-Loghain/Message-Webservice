# WebService Project

This project is a web service running in Docker containers using Docker Compose. It includes a .NET backend, a PostgreSQL database, and an Nginx server that serves multiple clients.

## Project Overview
The project consists of the following services:
- **Nginx (Static Files Server)**
- **.NET Web Service** (Backend API)
- **PostgreSQL Database** (Relational Database)

## Clients
The frontend clients are served by Nginx and are accessible at:
- `sendClient`: `http://localhost:8081/sendClient.html`
- `historyClient`: `http://localhost:8081/historyClient.html`
- `websocketClient`: `http://localhost:8081/websocketClient.html`

## Running the Project
To run the project locally using Docker Compose, use the following command:

```sh
docker-compose up
```

This will start all services.

## Docker Compose Services
### 1. **Database (PostgreSQL)**
- Runs PostgreSQL 15
- Exposed on `localhost:5416` (mapped to internal `5432`)

### 2. **.NET Web Service**
- Runs a .NET 8 API
- Exposed on `localhost:5109` (mapped to internal `8080`)

### 3. **Nginx Server**
- Serves static client files
- Accessible at `http://localhost:8081/`

## Features
- Clients can send messages with text (max 128 characters) and an ordinal number.
- The server assigns a timestamp to each message, stores it in the database, and forwards it to the WebSocket client.
- The WebSocket client listens for incoming messages and displays them in real-time.
- The history client allows users to retrieve messages sent within the last 10 minutes.
- Logging is implemented for monitoring application activity.
- Swagger documentation is available for REST API.

## Logging
Logs are stored in the logs/ directory inside the container. Log output includes:

- Incoming messages
- Database interactions
- WebSocket events

