# Jobify API Gateway

Welcome to the **Jobify API Gateway**, the central entry point for the Jobify ecosystem. This gateway is designed to manage users, sessions, and authentication while orchestrating requests across multiple specialized microservices.

## 🌌 Ecosystem Overview

Jobify is a distributed ecosystem of services designed to streamline the career management and recruitment lifecycle. The Gateway serves as the "brain" and security layer, handling:

- **Identity & Access Management (IAM)**: Centralized user registration, login, and profile management.
- **Session Orchestration**: Managing stateful and stateless sessions across the ecosystem.
- **Service Delegation**: Routing and delegating requests to downstream services.

### Downstream Services (The "Sub-Services")

The Gateway is built to eventually delegate domain-specific operations to:
- **ecom**: Job posting and candidate sourcing.
- **matching**: Job matching algorithms and recommendations.
- **roadmap**: Career path visualization and skill mapping.
- **matching ai**: Advanced AI-driven intelligence for recruitment.

---

## 🏗️ Architecture

The Gateway acts as a thin, highly efficient layer between clients and backend logic.

```pgsql
Web/Mobile Client
└─> Jobify API Gateway
    ├─ Auth & Registration
    ├─ User Management
    ├─ Session Manager
    ├─ E-com Service
    ├─ Matching Service
    ├─ Roadmap Service
    └─ Matching AI Service
```

---

## 🔐 Authentication & Security

All interactions with the Jobify ecosystem are secured via the Gateway. We use a combination of hashing for sensitive data and encryption for personal identifiers.

### Data Security Policy

- **User Identifiers**: Emails and other identifiers are securely hashed and encrypted to protect privacy.
- **Passwords**: Stored using strong one-way hashing algorithms.
- **Personal Information**: Names and contact details are encrypted using advanced encryption standards to ensure confidentiality.

---

## 📡 API Reference

The API follows RESTful conventions and returns a standardized response envelope.

### Standard Response Envelope

```json
{
  "success": true,
  "messageId": "string (UUID)",
  "message": "Human readable summary",
  "details": [],
  "data": { ... },
  "timestamp": "ISO8601"
}
```

---

## 🛠️ Development & Tooling

- **Scalars / OpenAPI**: Interactive documentation is available at `/openapi/v1.json` (or via the Scalar UI in development).
- **Structured Logging**: Every request is tagged with a `TraceId` for debugging.
- **Global Error Handling**: Consistent error responses via custom middleware.
