using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISmokingDetection.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntityIdsAndAddFaceReferenceMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Staff",
                newName: "StaffId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Participants",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Locations",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cameras",
                newName: "CameraId");

            migrationBuilder.CreateTable(
                name: "FaceReferenceMetadata",
                columns: table => new
                {
                    FaceReferenceMetadataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceReferenceMetadata", x => x.FaceReferenceMetadataId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaceReferenceMetadata");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Staff",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "Participants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Locations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CameraId",
                table: "Cameras",
                newName: "Id");
        }
    }
}
