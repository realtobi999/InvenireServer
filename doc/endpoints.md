# Endpoints Overview

## Table of Contents

- **[Server Endpoints](#server-endpoints)**
  - [GET /api/server/health-check](#get---apiserverhealth-check)
  - [GET /api/server/auth-check](#get---apiserverauth-check)

## Server Endpoints

### `GET - /api/server/health-check`

Checks if the server is running properly.

### Parameters

- None

### Headers

- None

### Returns

- `200` - Success.

### `GET - /api/server/auth-check`

Checks if the server's authentication system is working correctly.

### Parameters

- None

### Headers

- None

### Returns

- `200` - Success.
- `401` - Unauthorized.

## Employee Endpoints

### `POST - /api/auth/employee/register`

Registers a new employee account.

### Parameters

- None

### Headers

- None

### Request Body

```json
{
  "name": string,
  "email_address": string,
  "password": string,
  "password_confirm": string
}
```

### Returns

- `201` - Created.
- `400` - Bad Request.

### `POST - /api/auth/employee/login`

Authenticates an employee and returns a JWT on successful login.

### Parameters

- None

### Headers

- None

### Request Body

```json
{
  "email_address": string,
  "password": string
}
```

### Response Body

```json
{
  "token": string
}
```

### Returns

- `201` - Created.
- `401` - Unauthorized.
- `429` - Too Many Requests.

### `POST /api/auth/employee/email-verification/send`

Sends an email verification link to the authenticated employee.

### Parameters

- None

### Headers

- None

### Returns

- `204` - No Content.
- `400` - Bad Request.
- `401` - Unauthorized.

### `GET /api/auth/employee/email-verification/confirm`

Confirms the employee's email verification using the provided token.

### Parameters

- None

### Headers

- None

### Returns

- `204` - No Content.
- `400` - Bad Request.
- `401` - Unauthorized.
