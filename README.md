# Billing Service

The **Billing Service** is responsible for calculating the cost of EV charging sessions based on session data received from the Station Management Service. It acts as the system’s source of truth for billing logic and payment processing.

---

## Responsibilities

### MVP
- Consume charging session events:
  - `session_started`
  - `session_ended`
- Calculate the total amount to be billed based on:
  - Session duration
  - Pricing information provided by the Station Management Service
- Persist billing records for completed sessions

### Planned / Advanced
- Define supported payment methods for the platform
- Integrate **Stripe** for payment processing (test mode)
- Explore different charging models (e.g. per kWh, per minute, flat rate)
- Optional: support alternative payment instruments (e.g. prepaid / virtual cards)

---

## Architecture Context

- Receives events from **Station Management Service**
- Exposes billing results to other services (e.g. User Service, Notification Service)
- Runs as an independent, containerized .NET microservice

---

## Technology Stack

- **.NET 8**
- **ASP.NET Core**
- **Docker**
- **Kubernetes** (deployment managed externally)
- **Stripe** (planned)

---

## Local Development

### Prerequisites
- .NET SDK (version pinned in `global.json`)
- Docker (optional)

### Run locally
```bash
dotnet restore
dotnet run --project src/BillingService
```

---

## Build & Test

```bash
dotnet build
dotnet test
```

---

## Container Image

This service is built and published as a Docker image via CI/CD.  
Image building and deployment are handled outside this repository.

---

## Notes

- This service does **not** manage charging sessions directly.
- Pricing data is assumed to be provided by upstream services.
- Deployment configuration lives in the infrastructure repository.
