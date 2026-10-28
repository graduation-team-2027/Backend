using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISmokingDetection.Migrations
{
    public partial class AddCameraLocationAndParticipantFaceRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FaceReferenceMetadata_ParticipantId'
                      AND object_id = OBJECT_ID('dbo.FaceReferenceMetadata')
                )
                BEGIN
                    CREATE INDEX IX_FaceReferenceMetadata_ParticipantId
                    ON dbo.FaceReferenceMetadata (ParticipantId);
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Cameras_LocationId'
                      AND object_id = OBJECT_ID('dbo.Cameras')
                )
                BEGIN
                    CREATE INDEX IX_Cameras_LocationId
                    ON dbo.Cameras (LocationId);
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Cameras_Locations_LocationId'
                      AND parent_object_id = OBJECT_ID('dbo.Cameras')
                )
                BEGIN
                    ALTER TABLE dbo.Cameras
                    ADD CONSTRAINT FK_Cameras_Locations_LocationId
                    FOREIGN KEY (LocationId)
                    REFERENCES dbo.Locations (LocationId)
                    ON DELETE CASCADE;
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_FaceReferenceMetadata_Participants_ParticipantId'
                      AND parent_object_id = OBJECT_ID('dbo.FaceReferenceMetadata')
                )
                BEGIN
                    ALTER TABLE dbo.FaceReferenceMetadata
                    ADD CONSTRAINT FK_FaceReferenceMetadata_Participants_ParticipantId
                    FOREIGN KEY (ParticipantId)
                    REFERENCES dbo.Participants (ParticipantId)
                    ON DELETE CASCADE;
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_FaceReferenceMetadata_Participants_ParticipantId'
                      AND parent_object_id = OBJECT_ID('dbo.FaceReferenceMetadata')
                )
                BEGIN
                    ALTER TABLE dbo.FaceReferenceMetadata
                    DROP CONSTRAINT FK_FaceReferenceMetadata_Participants_ParticipantId;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Cameras_Locations_LocationId'
                      AND parent_object_id = OBJECT_ID('dbo.Cameras')
                )
                BEGIN
                    ALTER TABLE dbo.Cameras
                    DROP CONSTRAINT FK_Cameras_Locations_LocationId;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FaceReferenceMetadata_ParticipantId'
                      AND object_id = OBJECT_ID('dbo.FaceReferenceMetadata')
                )
                BEGIN
                    DROP INDEX IX_FaceReferenceMetadata_ParticipantId
                    ON dbo.FaceReferenceMetadata;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Cameras_LocationId'
                      AND object_id = OBJECT_ID('dbo.Cameras')
                )
                BEGIN
                    DROP INDEX IX_Cameras_LocationId
                    ON dbo.Cameras;
                END;
                """);
        }
    }
}
