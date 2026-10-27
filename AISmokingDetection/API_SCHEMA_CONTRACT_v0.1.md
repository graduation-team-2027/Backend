
# AISmokingDetection
## API / Schema Contract v0.1

**Owner:** BE-A — Asmaa Tarek
**Status:** Draft — Pending BE-A / BE-B Agreement

### 1. Purpose
Define the initial shared API and database contract
for the Smoking and Vaping Detection project.

### 2. BE-A Responsibilities
- Staff access and roles
- Participant enrollment metadata
- Camera and Location lookups
- Protected face-reference metadata

### 3. BE-B Responsibilities
- Incident lifecycle
- Evidence metadata
- ProcessingJob
- ReviewHistory and review workflow

### 4. Shared Decisions — Pending Agreement
- Primary key format: int or Guid
- Exact entity and field names
- Foreign keys and relationships
- Enum values and status labels
- UTC timestamp format
- Standard API error response
- Shared DbContext and migration ownership

### 4.1 Proposed ID and Field Naming Convention

**Status: Pending BE-A / BE-B Agreement**

- Entity primary keys: Pending decision (`int` or `Guid`).
- Primary key property: `Id`.
- Foreign key properties: use the related entity name followed by `Id`, for example `LocationId` and `ParticipantId`.
- Property names: use PascalCase in C#.
- Entity names: use singular names, for example `Staff`, `Participant`, `Camera`, and `Location`.
- Both backend owners must agree on the final field names and relationships before implementation changes.

### 4.2 Proposed Enum Values and Status Labels

**Status: Pending BE-A / BE-B Agreement**

- Staff roles: `Administrator`, `Reviewer`.
- Active status: `Active`, `Inactive`.
- Incident status values: To be agreed with BE-B.
- ProcessingJob status values: To be agreed with BE-B.
- Review status values: To be agreed with BE-B.
- Enum values and status labels must be consistent across the API and database.
- Final values must be approved by both backend owners before implementation.

### 4.3 Proposed UTC Timestamp Format

**Status: Pending BE-A / BE-B Agreement**

- All timestamps must be stored and exchanged in UTC.
- In C#, use `DateTime` with UTC values or an agreed UTC-safe alternative.
- API timestamps should use ISO 8601 format with a UTC indicator (`Z`).
- Example: `2026-10-23T08:30:00Z`.
- Both backend owners must agree on the timestamp conventions before implementation.

### 4.4 Proposed Standard API Error Response

**Status: Pending BE-A / BE-B Agreement**

All API errors should follow a consistent JSON structure.

Example:

{
  "statusCode": 400,
  "message": "Invalid request.",
  "errors": [
    "ParticipantId is invalid."
  ],
  "traceId": "example-trace-id"
}

Proposed HTTP status codes:
- 400 Bad Request: Invalid input.
- 401 Unauthorized: Authentication is required.
- 403 Forbidden: The user does not have permission.
- 404 Not Found: The requested resource does not exist.
- 500 Internal Server Error: An unexpected server error occurred.

Do not expose passwords, secrets, or sensitive face-reference metadata in error responses.
The final response shape must be agreed upon by BE-A and BE-B.

### 4.5 Proposed Staff Roles

**Status: Pending BE-A / BE-B Agreement**

#### Administrator
- Manage staff accounts and roles.
- Manage camera and location configuration.
- Access participant enrollment and lookup operations.

#### Reviewer
- Access authorized camera and participant lookup operations.
- Access incident review operations according to the agreed BE-B permissions.
- Cannot manage staff accounts or change staff roles.

#### General Rules
- Every protected operation must require authentication.
- Authorization must be enforced on the server.
- The final permissions must be agreed upon by BE-A and BE-B.
- Staff accounts must remain separate from monitored Participant records.

### 4.6 Proposed Permissions Matrix

**Status: Pending BE-A / BE-B Agreement**

| Operation | Administrator | Reviewer |
|---|---|---|
| View camera lookup | Allow | Allow |
| View participant lookup | Allow | Allow |
| Enroll participant metadata | Allow | Pending Agreement |
| Manage camera/location configuration | Allow | Deny |
| Create/manage staff accounts | Allow | Deny |
| Change staff roles | Allow | Deny |
| Review incidents | Allow | Allow, subject to BE-B rules |

Notes:
- All permissions must be enforced server-side.
- Unauthenticated requests must be rejected.
- Unauthorized roles must receive HTTP 403.
- Final permissions require agreement with BE-B and the team.

### 5. Initial API Endpoints
- GET /health
- GET /api/Staff

### 6. Security Notes
- Never expose PasswordHash in API responses.
- Do not store plain-text passwords.
- Protect face-reference metadata and storage references.
- Keep Staff accounts separate from Participants.

### 7. Status
This is a draft only.
Do not treat proposed fields or endpoints as approved
until both backend owners agree.

## 8. Sample Camera and Participant Lookup Records

**Status: Proposed Demo Data**

These records are fictional and for local development only.

### Camera Lookup Example

Request:
GET /api/Cameras

Example response:
[
  {
    "id": 1,
    "name": "Demo Camera 01",
    "cameraCode": "CAM-DEMO-001",
    "locationId": 1,
    "isActive": true
  }
]

### Participant Lookup Example

Request:
GET /api/Participants

Example response:
[
  {
    "id": 1,
    "fullName": "Demo Participant 01",
    "participantCode": "PART-DEMO-001",
    "isActive": true
  }
]

### Integration Notes
- These are sample contract examples, not confirmed working endpoints.
- WEB and MOB must use the final routes and response fields agreed by BE-A and BE-B.
- Do not include passwords, raw face images, or biometric templates in lookup responses.
- Confirm the final examples with the frontend and mobile teams before integration.
