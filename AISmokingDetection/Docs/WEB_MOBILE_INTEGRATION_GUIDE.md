# AISmokingDetection
# Web & Mobile Integration Guide

**Document Owner:** BE-A — Asmaa Tarek\
**Audience:** Web Development Team & Mobile Development Team\
**Backend:** ASP.NET Core Web API — .NET 10\
**Status:** Draft — For Frontend/Mobile Integration\
**API Base URL (Local):** `http://localhost:5053`

---

## 1. Purpose

This document provides the information required by the Web and Mobile teams to integrate with the AISmokingDetection backend.

It covers:

- API connection and local setup.
- Authentication and access tokens.
- Staff roles and permissions.
- Available API endpoints.
- Request and response examples.
- Participant enrollment.
- Camera and participant lookup.
- Face-reference image upload.
- Error handling.
- Security requirements.
- Integration dependencies and pending decisions.

The backend API is the source of truth for authentication, authorization, and data validation.

---

## 2. Backend Connection

### 2.1 Local Development

Start the API from the repository root:

```bash
dotnet run --project ".\AISmokingDetection\AISmokingDetection.csproj" --launch-profile http
```

Local API URL:

```text
http://localhost:5053
```

Health check:

```http
GET /health
```

Expected response:

```json
{
  "status": "Healthy",
  "service": "AISmokingDetection"
}
```

### 2.2 Connecting from Web

If the Web application runs on the same computer, use:

```text
http://localhost:5053
```

The Web team must coordinate with BE-A if browser CORS configuration is required.

### 2.3 Connecting from Mobile

If the mobile application runs on the same computer as the API, it can use the local development address where supported.

For a separate physical phone:

- Do not use `localhost`; it points to the phone itself.
- Use the development computer's reachable LAN IP address.
- Keep port `5053`.
- Ensure the API listens on a reachable network interface.
- Configure Windows Firewall if necessary.
- Use an approved secure development network.

Do not expose the development API publicly.

---

## 3. Authentication

The backend uses JWT Bearer authentication for protected operations.

### 3.1 Login

```http
POST /api/staff/login
Content-Type: application/json
```

Send the local test account credentials using the request format agreed with BE-A.

The successful login response provides an access token.

The frontend must not hardcode credentials or tokens.

### 3.2 Authorization Header

For protected API requests, include:

```http
Authorization: Bearer <access-token>
```

Example:

```http
GET /api/Camera
Authorization: Bearer <access-token>
```

### 3.3 Token Handling

Web and Mobile teams should:

- Store tokens using an appropriate secure mechanism.
- Attach the token to every protected request.
- Never print tokens in production logs.
- Never commit tokens or credentials to source control.
- Handle expired or invalid tokens.
- Return the user to the login flow when authentication is no longer valid.

**Token expiration and refresh-token behavior must be confirmed with BE-A before implementation.**

---

## 4. Staff Roles and Permissions

The backend currently defines two proposed staff roles:

- `Administrator`
- `Reviewer`

Authorization must be enforced by the backend. Hiding a button in the UI is not sufficient security.

### 4.1 Administrator

The Administrator is responsible for system administration.

Proposed permissions:

- View camera information.
- View participant information.
- Enroll participant metadata.
- Manage camera and location configuration.
- Create and manage staff accounts.
- Assign and change staff roles.
- Access incident review operations.

### 4.2 Reviewer

The Reviewer is responsible for reviewing authorized system information and incidents.

Proposed permissions:

- View authorized camera information.
- View authorized participant information.
- Access incident review operations according to BE-B rules.
- Cannot create or manage staff accounts.
- Cannot change staff roles.
- Cannot manage camera or location configuration.

### 4.3 Permissions Matrix

| Operation | Administrator | Reviewer |
|---|---|---|
| View camera lookup | Allow | Allow |
| View participant lookup | Allow | Allow |
| Enroll participant metadata | Allow | Pending |
| Manage cameras and locations | Allow | Deny |
| Create/manage staff | Allow | Deny |
| Change staff roles | Allow | Deny |
| Review incidents | Allow | Subject to BE-B rules |

**Important:** This is the proposed permissions matrix, not final approval. Reviewer enrollment and detailed incident-review permissions require confirmation from BE-A and BE-B.

### 4.4 Expected Authorization Responses

| Situation | Expected HTTP Status |
|---|---:|
| Valid authenticated request with permission | 200 / appropriate success status |
| Missing or invalid token | 401 Unauthorized |
| Authenticated user without permission | 403 Forbidden |

The UI should display a suitable message instead of exposing internal backend details.

---

## 5. Available API Endpoints

The following endpoints are documented in the current local access guide.

| Method | Endpoint | Purpose | Authentication |
|---|---|---|---|
| GET | `/health` | Check API health | No |
| POST | `/api/staff/login` | Staff login | No |
| GET | `/api/Camera` | Retrieve camera lookup | Yes |
| GET | `/api/Participant` | Retrieve participant lookup | Yes |
| GET | `/api/staff/me` | Retrieve current staff information | Yes |
| POST | `/api/Participant` | Enroll participant metadata | Yes |
| POST | `/api/FaceReference/{participantId}` | Upload participant face reference | Yes |

The API also has a staff-management endpoint documented and tested for Administrator access:

```http
GET /api/Staff
```

Staff creation and management operations must follow the permissions enforced by the backend.

**Important:** Do not assume that every proposed endpoint in the schema contract is already implemented. Confirm route names, HTTP methods, request fields, and response fields with BE-A before integrating additional operations.

---

## 6. Current Staff Information

The current staff member can be retrieved using:

```http
GET /api/staff/me
Authorization: Bearer <access-token>
```

This endpoint can be used to identify the authenticated staff member and determine the role information returned by the backend.

The frontend should use the returned role to control the displayed navigation and available actions.

The backend remains responsible for enforcing the actual permissions.

---

## 7. Camera Lookup

Request:

```http
GET /api/Camera
Authorization: Bearer <access-token>
```

A successful response returns available camera lookup records.

Example demo record:

```json
{
  "cameraId": 1,
  "name": "Demo Camera",
  "cameraCode": "DEMO-C001",
  "locationId": 1,
  "locationName": "Demo Location",
  "isActive": true
}
```

The example is illustrative. Use the actual response returned by the running API as the final integration reference.

Possible UI uses:

- Display available cameras.
- Select a camera when viewing related information.
- Display camera status.
- Display the associated location.

Inactive cameras may be excluded from lookup results.

---

## 8. Participant Lookup

Request:

```http
GET /api/Participant
Authorization: Bearer <access-token>
```

A successful response returns available participant lookup records.

Example:

```json
{
  "participantId": 1,
  "fullName": "Demo Participant",
  "participantCode": "PART-DEMO-001",
  "isActive": true
}
```

The example is illustrative, not a guaranteed response schema.

Possible UI uses:

- Display participant information.
- Search or select a participant.
- Open participant details.
- Start an authorized enrollment workflow.

Do not expose raw face images or biometric templates in participant lookup screens.

---

## 9. Participant Enrollment

Endpoint:

```http
POST /api/Participant
Authorization: Bearer <access-token>
Content-Type: application/json
```

Example request:

```json
{
  "fullName": "Test Participant",
  "participantCode": "TEST-P002"
}
```

Expected responses:

| Status | Meaning |
|---|---|
| 201 Created | Participant successfully enrolled |
| 400 Bad Request | Required fields or input are invalid |
| 401 Unauthorized | Missing or invalid token |
| 409 Conflict | ParticipantCode already exists |

### Enrollment Notes

- Enrollment currently stores participant metadata only.
- It does not upload a face image.
- Participant codes must be unique.
- The backend validates incoming data.

The UI should provide validation messages and handle duplicate participant codes.

---

## 10. Face Reference Upload

Endpoint:

```http
POST /api/FaceReference/{participantId}
Authorization: Bearer <access-token>
Content-Type: multipart/form-data
```

The file field must be named:

```text
image
```

Supported extensions in the current implementation:

- JPG
- PNG

The backend also checks the file signature.

### Example Workflow

1. Enroll the participant or select an existing participant.
2. Obtain the participant ID.
3. Open the face-reference upload interface.
4. Select an image.
5. Send a multipart/form-data request.
6. Handle the response.

Expected successful status:

```text
201 Created
```

### Important Notes

- The participant must exist.
- The image is handled by the backend.
- Metadata is stored in the database.
- The current implementation uses local file storage.
- This does not mean cloud storage integration is available.
- File signature checks do not guarantee that an image is completely valid or safe.

The Web/Mobile team must not send face images as ordinary participant JSON fields.

---

## 11. Error Handling

The frontend should handle HTTP responses consistently.

| Status | Meaning | Suggested UI Behavior |
|---|---|---|
| 400 | Bad Request | Display validation errors |
| 401 | Unauthorized | Redirect to login or request authentication |
| 403 | Forbidden | Display an access-denied message |
| 404 | Not Found | Display a not-found message |
| 409 | Conflict | Display duplicate-data message |
| 500 | Internal Server Error | Display a general error message |

The proposed standard backend error response is:

```json
{
  "statusCode": 400,
  "message": "Invalid request.",
  "errors": [
    "ParticipantId is invalid."
  ],
  "traceId": "example-trace-id"
}
```

This response format is still pending agreement between BE-A and BE-B. The frontend should avoid depending on this exact structure until it is approved.

---

## 12. Recommended Frontend Structure

The following is a suggested organization. Teams may adapt it to their chosen frameworks.

### Authentication

- Login screen.
- Token handling.
- Logout.
- Unauthorized response handling.

### Dashboard

- Current staff information.
- Role-based navigation.
- Available system sections.

### Camera Management / Lookup

- Camera list.
- Camera details.
- Location information.
- Configuration actions for authorized users only.

### Participant Management

- Participant list.
- Participant details.
- Participant enrollment.
- Face-reference upload.

### Incident Review

- Incident list and details.
- Evidence viewing.
- Review actions.

Incident and evidence endpoints, response fields, and review permissions must be coordinated with BE-B before implementation.

### Staff Management

- Staff list.
- Create staff account.
- Assign role.
- Update staff information.

These actions should be available only to authorized Administrators.

---

## 13. Security Requirements

Web and Mobile teams must follow these requirements:

1. Never store plain-text passwords.
2. Never expose JWT signing keys or backend secrets.
3. Never commit credentials or tokens.
4. Never expose PasswordHash in UI responses.
5. Do not expose raw face images or biometric templates in lookup responses.
6. Protect face-reference images and their storage references.
7. Do not assume that hiding a UI action prevents unauthorized access.
8. Do not trust role information sent by the client as proof of permission.
9. Handle 401 and 403 responses correctly.
10. Do not expose the development API publicly without an approved security configuration.

---

## 14. Integration Dependencies and Pending Decisions

The following points require confirmation before final integration:

| Item | Responsible Party | Status |
|---|---|---|
| Final entity IDs and field names | BE-A / BE-B | Pending agreement |
| Final API request and response contracts | BE-A / BE-B | Pending agreement |
| Reviewer enrollment permission | BE-A / Team | Pending |
| Detailed incident-review permissions | BE-B | Pending |
| Incident and evidence endpoints | BE-B | Pending integration |
| ProcessingJob endpoints and status values | BE-B | Pending integration |
| ReviewHistory workflow | BE-B | Pending integration |
| Standard error response | BE-A / BE-B | Pending agreement |
| JWT expiration and refresh behavior | BE-A | To be confirmed |
| Production API URL and deployment configuration | Backend team | Pending |

---

## 15. Integration Checklist

### Initial Setup

- [ ] Obtain the backend repository or API access instructions.
- [ ] Start the API locally.
- [ ] Verify `GET /health`.
- [ ] Configure the API base URL.
- [ ] Test the login endpoint.
- [ ] Obtain a valid access token.
- [ ] Test a protected endpoint.

### Authentication and Authorization

- [ ] Implement login.
- [ ] Attach Bearer tokens to protected requests.
- [ ] Implement logout.
- [ ] Handle 401 responses.
- [ ] Handle 403 responses.
- [ ] Retrieve the current staff member.
- [ ] Implement role-based navigation.
- [ ] Hide unauthorized actions in the UI while retaining server-side authorization.

### Camera and Participant Features

- [ ] Display camera lookup results.
- [ ] Display participant lookup results.
- [ ] Implement participant enrollment.
- [ ] Handle duplicate participant codes.
- [ ] Implement face-reference upload.
- [ ] Handle upload errors.

### Final Integration

- [ ] Confirm all request and response fields with BE-A.
- [ ] Confirm role permissions with BE-A and BE-B.
- [ ] Integrate incident and evidence features after BE-B provides the contract.
- [ ] Test with Administrator and Reviewer accounts.
- [ ] Verify that unauthorized operations are rejected by the API.

---

## 16. Important References

- `LOCAL_ACCESS.md` — Local API startup, authentication, and access instructions.
- `API_SCHEMA_CONTRACT_v0.1.md` — Initial shared API, schema, and permissions proposal.
- `Docs/BE-B-Setup-and-Handover.md` — Database and migration handover information.
- `Docs/Face-Reference-Storage-Design.md` — Face-reference storage design.
- `Docs/AISmokingDetection/OUTSTANDING_ENROLLMENT_AND_PRIVACY_REQUIREMENTS.md` — Outstanding enrollment and privacy requirements.

---

## 17. Final Notes

This document is intended to help the Web and Mobile teams begin integration using the currently available backend functionality.

The actual running API and the final approved contracts take precedence over illustrative examples in this guide.

Any endpoint, field, permission, or workflow marked as pending must be confirmed with the responsible backend owner before being treated as finalized.
