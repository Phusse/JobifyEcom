# 🚀 JobifyEcom API

JobifyEcom API provides a robust platform for managing jobs, workers, and applications. It supports role-based access, allowing users, workers, admins, and superadmins to interact with the system according to their privileges. The API is secured via JWT authentication and follows a consistent standard response format to simplify integration and error handling.

## 🌐 Base URLs

* **Development**: `http://localhost:5122`
* **Production**: `n/a`

## 🔐 Authentication

* **Method**: Bearer JWT via `Authorization` header

## 👥 Roles

* **`user`** 👤 → Standard account for individuals who can **post and manage jobs**.
* **`worker`** 🧑‍🏭 → Accounts for professionals who can **create worker profiles, showcase skills, and apply to jobs**.
* **`admin`** 🛡️ → Elevated privileges, able to **manage users, jobs, and applications** across the platform.
* **`superadmin`** 👑 → Full system access, including **management of admins and superadmins**.

## 🧾 Standard Response

```json
{
  "traceId": "12345",
  "success": true,
  "messageId": "USER_FOUND",
  "message": "Operation successful",
  "details": [],
  "timestamp": "2023-04-01T12:34:56Z",
  "data": {}
}
```

### **Description:**

* **traceId** : Unique identifier for tracing requests across services.
* **success** : `true` if the operation succeeded, `false` if it failed.
* **messageId** : Machine-readable code to identify the response (e.g., `USER_FOUND`, `AUTH_UNAUTHORIZED`).
* **message** : Human-readable message describing the result.
* **details** : Contextual info such as validation issues, warnings, or hints. Used in both success and failure responses.
* **timestamp** : UTC timestamp when the response was generated.
* **data** : The payload returned by the API.

## ✨ Features Overview

* **🔑 Authentication** : Register, log in, refresh, and log out sessions.
* **👤 Users** : Manage profiles, search/browse other users, update or delete accounts, and handle credentials.
* **💼 Jobs (User role)** : Post, update, delete, and browse job listings.
* **🧑‍🏭 Workers (Worker role)** : Create/manage worker profiles, browse jobs, apply, and track applications.
* **📄 Applications** : Workers can apply to jobs; users can view, accept, or reject applications.
* **🗂️ Metadata** : Retrieve system enums (e.g., roles, statuses, verification states) for use in dropdowns, filters, and validation without hardcoding values.

## ⚠️ Errors & Validation

* Validation issues or business logic errors are returned in `errors[]`.
* Unexpected errors include a `traceId` for easier troubleshooting.

## 📚 API Docs

* Auto-generated **OpenAPI spec**
* Interactive explorer available in **development**
