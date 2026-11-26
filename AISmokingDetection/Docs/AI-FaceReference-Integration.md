# AI Face Reference Integration Contract

## 1. Scope

This document defines the Backend APIs used by the AI service to:
- Retrieve a Face Reference image for enrollment.
- Retrieve an enrolled Face Reference image for gallery access.
- Report the enrollment processing result to the Backend.

The Backend owns participant metadata, storage access, authorization, and enrollment status.
The AI service owns image-quality validation, face processing, embedding generation, and gallery processing.

## 2. Authentication

Two independent shared secrets are used.

### AI image access

Header:

`X-AI-Service-Key: <shared-secret>`

Configuration key:

`AI:ServiceKey`

### Enrollment callback

Header:

`X-AI-Callback-Key: <shared-secret>`

Configuration key:

`AI:CallbackKey`

The two secrets are separate and must be shared securely.
Never commit either secret to source control or include it in logs, API examples, or public documentation.

## 3. Enrollment image endpoint

`GET /api/ai/face-references/{id}/enrollment-image`

Purpose:
Retrieve an image for enrollment processing.

Requirements:
- The Face Reference metadata record must exist.
- The Face Reference must be active.
- Its Participant must be active.
- Its enrollment status must be `Pending`.
- The request must include a valid `X-AI-Service-Key`.

Successful response:
- HTTP 200
- Binary image content.
- Content-Type is detected as `image/jpeg` or `image/png`.

The AI service should process the returned image and perform its required quality checks and embedding work.

## 4. Gallery image endpoint

`GET /api/ai/face-references/{id}/gallery-image`

Purpose:
Retrieve an enrolled image for gallery processing.

Requirements:
- The Face Reference metadata record must exist.
- The Face Reference must be active.
- Its Participant must be active.
- Its enrollment status must be `Ready`.
- The request must include a valid `X-AI-Service-Key`.

Successful response:
- HTTP 200
- Binary image content.
- Content-Type is detected as `image/jpeg` or `image/png`.

Revoked, inactive, Pending, or Failed references must not be used as gallery images.

## 5. Enrollment callback endpoint

`POST /api/ai/enrollment-callback`

Required header:

`X-AI-Callback-Key: <shared-secret>`

Content-Type:

`application/json`

Request example - successful enrollment:

```json
{
  "faceReferenceMetadataId": 123,
  "success": true,
  "failureReason": null
}
```

Request example - failed enrollment:

```json
{
  "faceReferenceMetadataId": 123,
  "success": false,
  "failureReason": "Face image quality is too low."
}
```

Rules:
- `faceReferenceMetadataId` must be a positive integer.
- `success` indicates the final enrollment outcome.
- When `success` is false, `failureReason` is required.
- `failureReason` must not exceed 1000 characters.
- On success, the Backend changes the status from `Pending` to `Ready`.
- On failure, the Backend changes the status from `Pending` to `Failed` and stores the failure reason.
- The AI service must only report success after all required validation and embedding work has completed successfully.
- The AI service must not report success merely because the image was downloaded or received.

## 6. Enrollment status lifecycle

| Status | Value | Meaning |
|---|---:|---|
| Pending | 0 | Waiting for AI enrollment processing |
| Ready | 1 | Enrollment completed successfully |
| Failed | 2 | Enrollment processing failed |
| Revoked | 3 | Reference has been revoked |

Expected transitions:

- Pending -> Ready
- Pending -> Failed
- Active reference -> Revoked through Backend revocation workflow

A revoked or inactive reference must not become Ready through a late callback.

## 7. Callback response behavior

| HTTP status | Meaning |
|---|---|
| 200 | Callback accepted or repeated with the same terminal status |
| 400 | Invalid request data |
| 401 | Missing or invalid callback key |
| 404 | Face Reference metadata record does not exist |
| 409 | Inactive, revoked, or conflicting terminal state |
| 503 | Callback key is not configured by the Backend |

Repeated callbacks with the same terminal status are treated as idempotent.
A callback that attempts to change an existing terminal status is rejected with HTTP 409.

## 8. Image access response behavior

| HTTP status | Meaning |
|---|---|
| 200 | Image returned successfully |
| 400 | Invalid metadata ID |
| 401 | Missing or invalid image service key |
| 404 | Metadata record or stored image not found |
| 409 | Reference or participant inactive, or status not allowed |
| 415 | Unsupported image format |
| 503 | Image service key is not configured by the Backend |

## 9. Security and storage boundaries

- The AI service receives image bytes only.
- The Backend does not expose StorageKey values through these endpoints.
- The Backend does not expose Cloudflare R2 credentials.
- The AI service must not access the storage bucket directly using Backend credentials.
- All shared secrets must be transferred through an approved secure channel.
- Do not include secrets in GitHub issues, pull requests, source code, or logs.

## 10. Current implementation and verification notes

Implemented:
- Enrollment callback endpoint.
- Separate callback and image-access authentication keys.
- Enrollment and gallery image access endpoints.
- Enrollment state and participant/reference activity checks.
- Local and Cloudflare R2 storage implementations.
- Local storage round-trip and storage path security tests.

Not yet verified:
- Live Cloudflare R2 upload and download.
- End-to-end integration with the AI service.
- AI gallery build and gallery version coordination.
- Confirmation that the AI service reports success only after validation and embedding are complete.

Database note:
The enrollment status migration was tested against the isolated migration-check database.
It has not been applied to the main AISmokingDetectionDB database.

This document describes the current Backend contract. Any contract changes must be coordinated between Backend and AI teams.
