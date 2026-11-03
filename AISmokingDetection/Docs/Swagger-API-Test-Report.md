# AISmokingDetection
# SWAGGER API TEST REPORT

## 1. ENVIRONMENT

- Framework: .NET 10
- Swagger: Swashbuckle.AspNetCore 10.3.1
- API URL: http://localhost:5053
- Swagger URL: http://localhost:5053/swagger
- Authentication: JWT Bearer
- Database: SQL Server LocalDB

## 2. SUCCESSFUL TESTS

### Health
- [PASS] `GET /health`
- Result: Healthy

### Camera
- [PASS] `GET /api/Camera`
- Returned active camera list successfully.
- [PASS] `GET /api/Camera/1/validate`
- Result: `isValid = true`
- [PASS] Inactive camera validation
- Result: HTTP 404 — Active camera not found.
- Inactive cameras are excluded from the camera list.

### Participant
- [PASS] `GET /api/Participant`
- Returned participant list successfully.
- [PASS] `POST /api/Participant`
- Created participant:
  - ID: 1002
  - Code: SWAGGER-P003
- [PASS] `POST /api/FaceReference/1002`
- Face reference image upload succeeded.
- FaceReferenceMetadataId: 2
- Metadata was saved successfully.
- [PASS] Inactive participant test
- Inactive participants are excluded from the participant list.

### Staff and Authentication
- [PASS] `GET /api/Staff/me`
- Returned authenticated Administrator information.
- [PASS] `GET /api/Staff`
- Returned staff list successfully for Administrator.
- [PASS] `POST /api/Staff`
- Created Reviewer:
  - ID: 1002
  - Email: swagger.reviewer@example.com
- [PASS] `POST /api/Staff/login`
- Reviewer login succeeded.
- Token Type: Bearer
- [PASS] `GET /api/Staff/me` using Reviewer token
- Returned authenticated Reviewer information.
- [PASS] Reviewer access to `GET /api/Staff`
- Result: HTTP 403 — Forbidden.
- [PASS] Reviewer attempt to create Staff
- Result: HTTP 403 — Forbidden.
- [PASS] Invalid login credentials
- Result: HTTP 401 — Unauthorized.
- [PASS] Inactive staff login
- Result: HTTP 401 — Unauthorized.
- Reactivated staff login succeeded.

## 3. ISSUES AND OBSERVATIONS

- JFIF images are currently rejected.
- JPG and PNG images are accepted.
- Face reference metadata was saved successfully.
- StackShift S2 integration is not implemented yet.
- Incident, Evidence, ProcessingJob and Review endpoints are pending integration with BE-B.
- Administrator and Reviewer authorization differences were tested.

## 4. PENDING TESTS

- Test invalid participant ID.
- Review remaining error responses.
- Complete StackShift S2 integration tests.
- Complete BE-B integration tests when the team starts integration.

## 5. SUMMARY

- Swagger UI is working.
- JWT Bearer authentication is working.
- Administrator and Reviewer permissions were tested.
- Camera and Participant active/inactive behavior was tested.
- Face reference metadata upload was tested successfully.
- Build succeeded with 0 warnings and 0 errors.

## 6. NOTES

- Exact HTTP status codes were not recorded for every test.
- This report covers Asmaa's BE-A work only.
- BE-B implementation and integration are tracked separately.
- No BE-B integration changes have been applied to AppDbContext.

==============================
END OF REPORT
