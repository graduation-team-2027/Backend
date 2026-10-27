# Day 3 - Database Migrations and Demo Seed Data

## Database Setup
- Database provider: SQL Server.
- Connection string key: DefaultConnection.
- Apply database migrations before running the application.

## Migrations
The project contains migrations for:
- Initial database schema.
- Entity-specific ID names and face reference metadata.
- Camera-to-Location and Participant-to-FaceReferenceMetadata relationships.

To apply database migrations, run from the solution root:

```powershell
dotnet ef database update --project ".\AISmokingDetection" --startup-project ".\AISmokingDetection"
```

## Demo Seed Data
The application seeds the following non-sensitive test records:
- Location: Demo Location
- Participant code: DEMO-P001
- Camera code: DEMO-C001

The demo camera is linked to the demo location through LocationId.

The seed logic checks for existing records before inserting them, preventing repeated inserts during normal sequential application restarts.

## Verification
- The solution build completed successfully.
- Database migrations were already up to date.
- Re-running the application performed existence checks without inserting duplicate demo records.
- The health endpoint was verified at /health.

Expected response:

```json
{
  "status": "Healthy",
  "service": "AISmokingDetection"
}
```

## Security Notes
- Demo data does not include real staff credentials.
- No face images or biometric templates are included in the demo seed data.
- Do not commit local secrets, real credentials, or biometric data to Git.
