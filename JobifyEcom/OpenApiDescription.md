# 🚀 JobifyEcom API — Quick Guide

## 🌐 Base URLs

* **Development**: `http://localhost:5122`
* **Production**: `https://localhost:5000`

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
  "message": "Success",
  "errors": [],
  "timestamp": "2023-04-01T12:34:56Z",
  "data": {}
}
```

## ✨ Features Overview

* **🔑 Authentication**: Register, log in, refresh, and log out sessions.
* **👤 Users**: Manage profiles, search/browse other users, update or delete accounts, and handle credentials.
* **💼 Jobs (User role)**: Post, update, delete, and browse job listings.
* **🧑‍🏭 Workers (Worker role)**: Create/manage worker profiles, browse jobs, apply, and track applications.
* **📄 Applications**: Workers can apply to jobs; users can view, accept, or reject applications.
* **🗂️ Metadata**: Retrieve system enums (e.g., roles, statuses, verification states) for use in dropdowns, filters, and validation without hardcoding values.

## ⚠️ Errors & Validation

* Validation issues are returned in `errors[]`
* Unexpected errors include a `traceId` for easier troubleshooting

## 📚 API Docs

* Auto-generated **OpenAPI spec**
* Interactive explorer available in **development**
