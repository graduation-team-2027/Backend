# AISmokingDetection - BE-B Setup and Handover

## 1. Current Verification Status

- Target framework: .NET 10 (`net10.0`).
- `dotnet build` succeeded with 0 warnings and 0 errors in the latest check.
- `dotnet ef migrations list` connected to the configured database and listed three migrations.
- The current database's migration history was readable.
- A fresh database setup has NOT yet been verified.

## 2. Required Configuration

Configure the API project's `appsettings.json` or an appropriate local configuration source with:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

Do not commit real credentials, signing keys, or private connection strings to source control.
Use User Secrets or an approved secure configuration mechanism for local development.

The JWT key must be Base64-encoded because the application decodes it during startup.

## 3. Database Migrations

Current migration sequence:

1. `20261023091759_InitialCreate`
2. `20261027113902_RenameEntityIdsAndAddFaceReferenceMetadata`
3. `20261027114846_AddCameraLocationAndParticipantFaceRelationships`

To inspect migrations:

```powershell
dotnet ef migrations list
```

To apply migrations, first verify that `DefaultConnection` points to the intended development or test database:

```powershell
dotnet ef database update
```

Do not run the update against a shared or production database without authorization and a backup plan.

The API startup code does not automatically call `Database.Migrate()`.
The database schema must be prepared before startup seeding runs.

## 4. Startup Seed Data

`DbInitializer.SeedAsync` creates demo records when matching records are absent:

- Location: `Demo Location`
- Participant code: `DEMO-P001`
- Camera code: `DEMO-C001`, associated with the demo location

The initializer does not create a Staff or Administrator account.
Confirm the approved staff-account provisioning process before demonstrating login.

## 5. Current Entity Relationships

- One Location can have multiple Cameras.
- Each Camera references a Location.
- One Participant can have multiple FaceReferenceMetadata records.
- Each FaceReferenceMetadata record references a Participant.

The current migration configuration uses cascade delete for both relationships.
Confirm that this behavior matches the project's data-retention and privacy requirements.

## 6. Enrollment and Privacy Items Requiring Confirmation

- Confirm who is authorized to enroll participants and upload reference images.
- Confirm consent, lawful purpose, retention period, deletion, and access procedures for face-reference data.
- Confirm where image files are stored and whether `StorageKey` may be returned by API responses.
- Confirm staff account creation, password handling, and administrator provisioning.
- Confirm whether cascade deletion is acceptable for participant and camera records.

These are open review items, not confirmed project decisions.

## 7. Migration Rollback Review

The `Down()` method in
`20261027114846_AddCameraLocationAndParticipantFaceRelationships.cs`
was updated to remove both foreign keys and both indexes added by `Up()`.

The project build succeeded after the file change.
Rollback behavior has not been executed against a separate test database.

## 8. Handover Acceptance Checklist

- [ ] BE-B can configure the required settings without receiving secrets through chat.
- [ ] BE-B can build the API.
- [ ] BE-B can create a separate empty test database and apply all migrations.
- [ ] Startup seeding succeeds on that test database.
- [ ] The approved staff-account provisioning process is documented.
- [ ] Enrollment and privacy decisions are documented.
- [ ] Rollback behavior is reviewed on a disposable test database.
- [ ] API integration and authorization expectations are agreed with BE-B.

## 9. Verified vs. Not Yet Verified

Verified:
- Latest build succeeded with zero warnings and zero errors.
- The current configured database was reachable.
- Three migration names were listed.
- The model snapshot file exists.

Not yet verified:
- Full setup from an empty database.
- Successful startup seeding on a fresh database.
- Rollback execution.
- BE-B's independent setup.
- Final enrollment and privacy policy decisions.

## Fresh Database Validation - Verified

- Test database: AISmokingDetectionDB_Test
- The database was created and all three migrations were applied successfully.
- Verified tables: Staff, Locations, Cameras, Participants, FaceReferenceMetadata.
- Verified foreign keys:
  - Cameras.LocationId -> Locations.LocationId
  - FaceReferenceMetadata.ParticipantId -> Participants.ParticipantId
- Seed verification:
  - Locations: 1
  - Participants: 1
  - Cameras: 1
  - Staff: 0
  - FaceReferenceMetadata: 0
- The seed results match the current DbInitializer implementation.
- The original database AISmokingDetectionDB was not targeted by the test migration or seed run.
- Not yet verified: full fresh-database rollback test, successful staff login on the test database, successful face-reference upload, and BE-B peer review.
