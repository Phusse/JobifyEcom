# 🚀 JobifyEcom API – Quick Overview

JobifyEcom provides **JWT auth**, user management, worker profiles, job postings, and applications.

**🌐 Base URLs**

* Development: `http://localhost:5122`
* Production: `https://localhost:5000`

**🔑 Auth & Roles**

* Scheme: `Bearer <access_token>`
* Roles: `Worker` needed for some endpoints

**📦 Standard Response**
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

---

## **Endpoints**

### **🛡 Auth (v1)**

* `POST /api/v1/auth/register` – Register
* `POST /api/v1/auth/login` – Login → access/refresh tokens
* `POST /api/v1/auth/refresh` – Refresh token
* `PATCH /api/v1/auth/logout` – Logout (auth required)

### **👤 Users (v1)**

* `GET /api/v1/users/me` – Current profile
* `GET /api/v1/users` – List/search users
* `GET /api/v1/users/{id}` – Get user
* `PATCH /api/v1/users/{id}` – Update
* `DELETE /api/v1/users/{id}` – Delete
* Email & password management endpoints

### **💼 Jobs**

* `POST /api/job` – Create job (Worker only)
* `GET /api/job` – List all jobs
* `GET /api/job/mine` – My jobs (Worker only)

### **🧑‍🏭 Worker**

* `POST /api/worker/profile` – Create profile
* `GET /api/worker/profiledetails` – Get own profile

### **📄 Job Applications**

* `POST /api/jobapplication` – Apply
* `GET /api/jobapplication/{id}` – Get application
* `PUT /api/jobapplication/{id}/accept` – Accept
* `PUT /api/jobapplication/{id}/reject` – Reject

---

**⚠ Errors & Validation**

* Aggregated `errors[]` for validation issues
* Middleware handles unhandled exceptions with `traceId`

**📚 Docs**

* OpenAPI JSON: `/openapi/v1.json`
* Interactive explorer: Dev only
