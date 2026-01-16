# JobifyEcom API

JobifyEcom API provides a robust platform for managing jobs, workers, and applications. It supports role-based access, allowing users, workers, admins, and superadmins to interact with the system according to their privileges. The API follows a consistent standard response format to simplify integration and error handling.

## Authentication

* **Method**: via custom header sent from the api gateway.

## 🧾 Standard Response

```json
{
  "success": true,
  "messageId": "USER_FOUND",
  "message": "Operation successful",
  "details": [...],
  "data": {...}
}
```

### **Description:**

* **success** : `true` if the operation succeeded, `false` if it failed.
* **messageId** : Machine-readable code to identify the response (e.g., `USER_FOUND`, `AUTH_UNAUTHORIZED`).
* **message** : Human-readable message describing the result.
* **details** : Contextual info such as validation issues, warnings, or hints. Used in both success and failure responses.
* **data** : The payload returned by the API.
