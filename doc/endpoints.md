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
