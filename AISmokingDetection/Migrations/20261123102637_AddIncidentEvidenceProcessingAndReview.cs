using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISmokingDetection.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentEvidenceProcessingAndReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    IncidentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CameraId = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IdentityStatus = table.Column<int>(type: "int", nullable: false),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.IncidentId);
                    table.ForeignKey(
                        name: "FK_Incidents_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "CameraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "ParticipantId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Evidences",
                columns: table => new
                {
                    EvidenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    EvidenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvailabilityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidences", x => x.EvidenceId);
                    table.ForeignKey(
                        name: "FK_Evidences_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "IncidentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingJobs",
                columns: table => new
                {
                    ProcessingJobId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ModelVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJobs", x => x.ProcessingJobId);
                    table.ForeignKey(
                        name: "FK_ProcessingJobs_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "IncidentId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReviewHistories",
                columns: table => new
                {
                    ReviewHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewHistories", x => x.ReviewHistoryId);
                    table.ForeignKey(
                        name: "FK_ReviewHistories_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "IncidentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewHistories_Staff_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_IncidentId",
                table: "Evidences",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CameraId",
                table: "Incidents",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ParticipantId",
                table: "Incidents",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_IncidentId",
                table: "ProcessingJobs",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHistories_IncidentId",
                table: "ReviewHistories",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHistories_ReviewerId",
                table: "ReviewHistories",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evidences");

            migrationBuilder.DropTable(
                name: "ProcessingJobs");

            migrationBuilder.DropTable(
                name: "ReviewHistories");

            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}
