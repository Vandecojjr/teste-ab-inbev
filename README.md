# AB InBev Test - Developer Evaluation

This repository contains the backend API developed for the technical evaluation. The entire ecosystem is containerized using Docker, making it easy to run and test without requiring complex setups.

## How to Run the Project

Make sure you have **Docker** and **Docker Compose** installed and running on your machine.

1. Bring up the application via Docker Compose. The `-d` parameter runs the containers in the background:
   ```bash
   docker-compose up -d --build
   ```

> **Note:** When running for the first time, the PostgreSQL database will execute the initialization script, which automatically applies the tables (via EF Core Migrations) and inserts the **test data** (one admin user and 10 mocked sales).

## How to Test the API

The application routes (such as Sales operations) are protected. Follow the step-by-step guide below to generate your JWT token and make requests.

### 1. Get the Token (Login)

Use Postman, Insomnia, or a similar tool to authenticate the default user created by the Seed script.

- **Method:** `POST`
- **URL:** `http://localhost:8080/api/auth`
- **Body (JSON):**
  ```json
  {
    "email": "admin@teste.com",
    "password": "admin123"
  }
  ```

The API will return a JSON similar to this:
```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "email": "admin@teste.com",
    "name": "admin",
    "role": "Admin"
  },
  "success": true,
  "message": "User authenticated successfully",
  "errors": []
}
```
**Copy the full value returned in the `token` field.**

### 2. Consume Protected Routes (Example: List Sales)

With the token in hand, you are authorized to consume the other API services.

In your HTTP client (Postman/Insomnia):
1. Create a `GET` request for the sales listing route:
   **URL:** `http://localhost:8080/api/sales`
2. Go to the **Authorization** section (or Headers) and select the **Bearer Token** type.
3. Paste the `token` generated in the previous step.
4. When sending the request, you should receive an HTTP `200 OK` with the 10 sales already registered in the database, properly paginated.

### 3. Access Swagger (Documentation)

If you prefer, all the API endpoint documentation is also available locally through the Swagger UI.

- **Swagger URL:** `http://localhost:8080/swagger`

> Remember that to test the restricted endpoints directly through Swagger, you will need to provide the Bearer Token, or test them externally via Insomnia/Postman.
