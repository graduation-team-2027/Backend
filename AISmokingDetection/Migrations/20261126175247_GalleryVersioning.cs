using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISmokingDetection.Migrations
{
    /// <inheritdoc />
    public partial class GalleryVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GalleryBuilds",
                columns: table => new
                {
                    GalleryBuildId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryBuilds", x => x.GalleryBuildId);
                });

            migrationBuilder.CreateTable(
                name: "GalleryStates",
                columns: table => new
                {
                    GalleryStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesiredVersion = table.Column<long>(type: "bigint", nullable: false),
                    AppliedVersion = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastBuildCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryStates", x => x.GalleryStateId);
                    table.CheckConstraint("CK_GalleryState_Singleton", "[GalleryStateId] = 1");
                });

            migrationBuilder.InsertData(
                table: "GalleryStates",
                columns: new[] { "GalleryStateId", "DesiredVersion", "AppliedVersion", "LastUpdatedAtUtc", "LastBuildCompletedAtUtc" },
                values: new object[] { 1, 0L, null, new DateTime(2026, 11, 26, 17, 52, 47, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryBuilds_Version",
                table: "GalleryBuilds",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryBuilds");

            migrationBuilder.DropTable(
                name: "GalleryStates");
        }
    }
}
