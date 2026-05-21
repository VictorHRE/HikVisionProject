#  Humand Access Control

 Humand Access Control is an integrated access management platform that connects Hikvision access control devices, a central orchestration API, Humand synchronization, and a React-based operational dashboard.

It is designed to support branch-level device operations, employee lifecycle management, fingerprint enrollment, event ingestion, and attendance reporting across a centralized architecture.

## Overview

This repository contains:

- `AccessControlAPI`: branch-level API responsible for communicating directly with Hikvision devices.
- `CentralHubAPI`: central API for authentication, branch catalog management, employee synchronization, and event intake.
- `Application`, `Domain`, `Infrastructure`, `Shared`: shared backend layers implementing the core business logic and architecture.
- `fingerPrintProject`: React + Vite frontend for login, employee operations, and branch management.

## Key Features

- Hikvision device connectivity and health checks
- Employee provisioning to access control devices
- Fingerprint enrollment and removal
- HTTP listener configuration for Hikvision event callbacks
- Local event persistence and attendance generation
- Centralized event logging and Humand synchronization
- JWT-based authentication in the central API
- Web interface for operational use

## Tech Stack

- `.NET 8` (`SDK 8.0.416`)
- `ASP.NET Core Web API`
- `Entity Framework Core 9`
- `SQL Server`
- `Swagger / OpenAPI`
- `JWT Bearer Authentication`
- `React 19`
- `TypeScript`
- `Vite 7`

## Repository Structure

```text
.
├─ AccessControlAPI.sln
├─ HikvisionApi/
│  ├─ AccessControlAPI/
│  ├─ CentralHubAPI/
│  ├─ Application/
│  ├─ Domain/
│  ├─ Infrastructure/
│  ├─ Shared/
│  └─ global.json
└─ fingerPrintProject/
```

## Architecture

The backend follows a layered architecture with clear domain separation.

### `Domain`

Defines the core entities and contracts for:

- employees
- devices
- access events
- attendance
- users
- catalogs and branch stores

It also contains important enums such as:

- `EmployeeStatus`: `ACTIVE`, `INACTIVE`, `DELETED`, `EXPIRED`, and others
- `EmployeeGender`: `male`, `female`, `unknown`
- `EmployeeType`: `normal`, `visitor`, `blackList`
- `SubEventType`: Hikvision event mappings such as `FINGER_PRINT_ACCESS_GRANTED`, `FINGER_PRINT_ACCESS_DENIED`, `DOOR_OPEN`, and `DOOR_CLOSE`

### `Application`

Implements use cases with a `Command/Query` pattern, including:

- creating, updating, retrieving, and deleting employees
- capturing and removing fingerprints
- retrieving device information
- configuring and reading the device HTTP listener
- processing inbound Hikvision events

### `Infrastructure`

Contains persistence and external integrations:

- `AmpmAccessControlContext`: local access control database context
- `AmPmCentralHubContext`: central database context
- HTTP clients for Hikvision, Humand, and internal APIs
- repositories for employees, devices, attendance, and synchronization workflows

### `Shared`

Provides lightweight command/query bus infrastructure and shared utilities.

## Main Components

### `AccessControlAPI`

`AccessControlAPI` is the branch-facing service that interacts directly with Hikvision access control hardware.

Core responsibilities:

- validating device connectivity via `ping`
- reading device metadata from `deviceInfo`
- configuring the Hikvision `HTTP Listener`
- managing employees on the device
- capturing and deleting fingerprints
- receiving `multipart/form-data` event payloads from Hikvision
- persisting events locally and converting fingerprint access events into attendance records

#### Event Flow

1. Hikvision sends an event to `api/event`.
2. `CreateEventCommandHandler` stores the event locally.
3. If the sub-event is `FINGER_PRINT_ACCESS_GRANTED`, the system evaluates whether it should register a `CheckIn` or a `CheckOut`.
4. The event is then forwarded to `CentralHubAPI` so it can be logged centrally and, when applicable, propagated to Humand as a `clock-in` or `clock-out` action.

### `CentralHubAPI`

`CentralHubAPI` acts as the orchestration layer for the whole platform.

Core responsibilities:

- authenticating users and issuing JWT tokens
- serving branch store data from `ConTienda`
- retrieving and synchronizing employees from Humand
- sending employees and fingerprint operations to branch-level APIs
- persisting centralized event logs
- sending `clock in` and `clock out` actions to Humand

### `fingerPrintProject`

`fingerPrintProject` is the operational frontend built with React.

Observed functionality:

- login flow
- employee listing and management workflows
- branch listing
- backend integration operations
- history view currently disabled in routes

The frontend stores `fp_token` in `localStorage` and primarily consumes `CentralHubAPI` through `VITE_API_URL`.

## Databases

### `AccessControlDB`

Used by `AccessControlAPI` through `AmpmAccessControlContext`.

Main inferred tables:

- `Devices`
- `Employees`
- `EventLogs`
- `EmployeeAttendances`

### Central database (`API` in the current configuration)

Used by `CentralHubAPI` through `AmPmCentralHubContext`.

Main tables:

- `ConTienda`
- `EmployeeHubs`
- `EventLogs`
- `Usuarios`
- `Catalogos`

## Configuration

### `AccessControlAPI/appsettings.json`

Relevant settings:

- `CentralHubCfg:IdStore`
- `CentralHubCfg:ApiUrl`
- `HikVisionDevice:Host`
- `HikVisionDevice:Username`
- `HikVisionDevice:Password`
- `AttendanceTime`
- `ConnectionStrings:DefaultConnection`

### `CentralHubAPI/appsettings.json`

Relevant settings:

- `Humand:ApiKey`
- `Humand:ApiUrl`
- `AccessControlCfg:ApiUrl`
- `Jwt:key`
- `Jwt:expired`
- `ConnectionStrings:DefaultConnection`

### `fingerPrintProject/.env`

Detected variable:

- `VITE_API_URL`

## Running Locally

### Backend

From the repository root:

```powershell
dotnet restore .\AccessControlAPI.sln
dotnet build .\AccessControlAPI.sln
dotnet run --project .\HikvisionApi\CentralHubAPI\CentralHubAPI.csproj
dotnet run --project .\HikvisionApi\AccessControlAPI\AccessControlAPI.csproj
```

### Frontend

```powershell
cd .\fingerPrintProject
npm install
npm run dev
```

## Main Endpoints

> Both APIs are configured to use lowercase routing, so the examples below are shown in lowercase.

### `AccessControlAPI`

#### Catalogs

- `GET /api/catalog/genders`
- `GET /api/catalog/statuses`
- `GET /api/catalog/employee-types`

#### Device

- `POST /api/device/connect`
- `GET /api/device/host-info`
- `GET /api/device/http-listener`
- `PUT /api/device/configure-http-listener`

Expected request body for listener configuration:

```json
{
  "port": 3000,
  "ipAddress": "192.168.1.10",
  "protocol": "http",
  "url": "/events"
}
```

#### Employees

- `POST /api/employee/get-employee-by-id`
- `POST /api/employee/get-employees`
- `POST /api/employee/add-employee`
- `POST /api/employee/update-employee`
- `DELETE /api/employee/delete-employee`
- `POST /api/employee/add-finger-print`
- `POST /api/employee/delete-finger-print`

Sample payload for employee create/update:

```json
{
  "identificationNumber": "001-000000-0000A",
  "name": "John",
  "lastName": "Doe",
  "email": "john.doe@company.com",
  "position": "Supervisor",
  "phone": "88888888",
  "branchId": 1,
  "status": "ACTIVE",
  "gender": "male",
  "birthDate": "1990-01-01T00:00:00",
  "beginDate": "2025-01-01T00:00:00",
  "endDate": "2030-01-01T00:00:00"
}
```

#### Events

- `POST /api/event`

This endpoint expects a `multipart/form-data` payload containing the `event_log` field sent by Hikvision.

### `CentralHubAPI`

#### Authentication

- `POST /api/authentication/login`
- `POST /api/authentication/gettoken`

Payload:

```json
{
  "id": 0,
  "name": "admin",
  "email": "admin@company.com",
  "password": "***"
}
```

#### Branches and Catalogs

- `GET /api/contienda/getcontienda`
- `GET /api/catalogo/getcatalogo`

#### Employees

- `POST /api/employee/get-employee-by-id`
- `GET /api/employee/get-employees`
- `POST /api/employee/get-employeeshumand`
- `POST /api/employee/add-employeetodevice`
- `POST /api/employee/update-employeetodevice`
- `POST /api/employee/add-fingerprintedemployee`
- `DELETE /api/employee/delete-fingerprintedemployee`

#### Events

- `POST /api/eventlog/add-eventlog`
- `POST /api/eventlog/add-eventlog-clockin`
- `POST /api/eventlog/add-eventlog-clockout`

## Recommended Startup Order

1. Start SQL Server.
2. Configure both `appsettings.json` files with real values.
3. Start `CentralHubAPI`.
4. Start `AccessControlAPI`.
5. Configure the Hikvision device HTTP listener to point to `AccessControlAPI`.
6. Start `fingerPrintProject`.

## Analysis Notes

- The solution builds successfully with `dotnet build`.
- No automated test projects were detected in the current solution.
- A versioned migration exists for `AmpmAccessControlContext` in `Infrastructure/Migrations`.
- No equivalent migration was found in the repository for `AmPmCentralHubContext`.
- The frontend currently targets `CentralHubAPI` through `VITE_API_URL`.
- In both APIs, `Program.cs` validates `host` and `port` from `appsettings.json`, although effective development URLs may also come from `launchSettings.json` or environment configuration.

## Operational Recommendations

- Do not commit credentials, API keys, passwords, or JWT secrets to source control.
- Move secrets to environment variables or `.NET User Secrets`.
- Document the exact Hikvision event contract received by `api/event` if this project will be integrated with other systems.
- Add automated tests for critical flows such as employee provisioning, fingerprint enrollment, event intake, and attendance registration.

## Current Functional Scope

Based on the current codebase, the system already supports this main operational flow:

1. authenticate users in the central hub,
2. retrieve branch information,
3. synchronize employees from Humand,
4. send employees to branch-level Hikvision devices,
5. capture and remove fingerprints,
6. receive device events,
7. register attendance and report it to the central hub and Humand.
