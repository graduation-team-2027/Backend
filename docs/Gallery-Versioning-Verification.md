# Gallery Versioning Verification

Date: 2026-11-26
Role: BE-A
Status: VERIFIED - ISOLATED DATABASE

## 1. Scope

Implemented backend Gallery Versioning for the shared global face gallery.

The backend tracks:
- DesiredVersion
- AppliedVersion
- LastUpdatedAtUtc
- LastBuildCompletedAtUtc
- Gallery build records

The backend does not perform AI gallery building. AppliedVersion must only be updated after AI confirms a successful build.

## 2. Versioning Rules

| Operation | Expected Version Change |
|---|---|
| Create Pending reference | No increment |
| Pending to Ready, active Participant | Increment |
| Pending to Failed | No increment |
| Repeated Ready callback | No additional increment |
| Revoke eligible Ready reference | Increment |
| Revoke Pending reference | No increment |
| Revoke Failed reference | No increment |

A reference is eligible for the gallery only when:
- FaceReferenceMetadata.IsActive is true.
- Status is Ready.
- Participant.IsActive is true.

## 3. Implementation

Implemented:
- IGalleryVersionService
- GalleryVersionService
- Atomic DesiredVersion increment using ExecuteUpdateAsync
- GalleryState singleton protection
- SQL Server RowVersion concurrency token
- GalleryBuild entity and version index
- Transactional version updates during successful enrollment callbacks and eligible revocations

## 4. Database Verification

Database:
AISmokingDetection_MigrationCheck

Operation:
READ ONLY verification, followed by isolated runtime tests.

Verified migrations:
- 20261023091759_InitialCreate
- 20261027113902_RenameEntityIdsAndAddFaceReferenceMetadata
- 20261027114846_AddCameraLocationAndParticipantFaceRelationships
- 20261123102637_AddIncidentEvidenceProcessingAndReview
- 20261126121230_AddFaceReferenceEnrollmentStatus
- 20261126175247_GalleryVersioning

Verified database objects:
- GalleryStates table exists.
- GalleryBuilds table exists.
- GalleryState singleton row exists with GalleryStateId = 1.
- Initial DesiredVersion = 0.
- Initial AppliedVersion = NULL.
- IX_GalleryBuilds_Version exists and is non-unique.
- CK_GalleryState_Singleton exists and enforces GalleryStateId = 1.
- RowVersion column exists.

## 5. Runtime Verification

Executed against:
AISmokingDetection_MigrationCheck

| Scenario | Expected | Actual | Result |
|---|---:|---:|---|
| Pending reference creation | 0 | 0 | PASS |
| Pending to Ready | 1 | 1 | PASS |
| Pending to Failed | 1 | 1 | PASS |
| Duplicate Ready callback | 1 | 1 | PASS |
| Revoke Ready reference | 2 | 2 | PASS |
| Revoke Pending reference | 2 | 2 | PASS |

HTTP results:
- Pending to Ready callback: 200
- Pending to Failed callback: 200
- Duplicate Ready callback: 200
- Revoke Ready reference: 200
- Revoke Pending reference: 200

Overall result:
ALL GALLERY VERSIONING TESTS PASSED

## 6. Test Cleanup

Temporary test records were removed.

GalleryState was restored to:
- DesiredVersion = 0
- AppliedVersion = NULL

The isolated test database was used.

The main database AISmokingDetectionDB was not modified.

## 7. Remaining Scope

Not implemented or verified by this task:
- AI Gallery Build execution.
- AI-to-backend build completion callback.
- Updating AppliedVersion after an actual successful AI build.
- End-to-end AI gallery integration.

These remain separate integration tasks.

## 8. Final Result

Gallery Versioning backend implementation:
PASS

Isolated database migration verification:
PASS

Runtime versioning tests:
PASS

Main database protection:
MAINTAINED
