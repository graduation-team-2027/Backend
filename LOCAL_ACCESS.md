# AISmokingDetection - Local Access

## Start the API

Run from the repository root:

    dotnet run --project ".\AISmokingDetection\AISmokingDetection.csproj" --launch-profile http

Base URL: http://localhost:5053

Health check: GET /health

## Local authentication

1. Send POST /api/staff/login with the local test account credentials.
2. Obtain the access token from the response.
3. For protected requests, send:
   Authorization: Bearer <access-token>

Never commit credentials, access tokens, or JWT signing keys.

## Protected lookup endpoints

- GET /api/Camera
- GET /api/Participant
- GET /api/staff/me

Expected results:
- 200 OK with a valid token.
- 401 Unauthorized without a valid token.

Empty lookup results are possible when no active records exist.

## Face reference upload

POST /api/FaceReference/{participantId}

Use multipart/form-data with a file field named `image`.
The current implementation allows JPG and PNG extensions and checks the file signature.
A successful upload returns 201 Created.

## Mobile development

- For Web or Mobile running on the same computer, use http://localhost:5053.
- For a separate phone, localhost refers to the phone itself.
- Configure the API base URL using the development computer's reachable LAN IP address and port 5053.
- Ensure the API listens on a reachable network interface and configure Windows Firewall as needed.
- Use an approved secure network setup; do not expose the development API publicly.
## Security notes

- Keep signing keys and credentials out of source control.
- Restrict access to face images.
- Basic signature checks do not prove that an image is fully valid or safe.
- Do not expose the local development API publicly without an approved security setup.

## Participant enrollment

POST /api/Participant

Requires a valid Bearer access token.

Content-Type: application/json

Example request:

    {
      "fullName": "Test Participant",
      "participantCode": "TEST-P002"
    }

Expected results:
- 201 Created when a participant is enrolled.
- 400 Bad Request when required fields are invalid.
- 409 Conflict when ParticipantCode already exists.
- 401 Unauthorized without a valid token.

Enrollment stores participant metadata only. It does not upload a face image.
