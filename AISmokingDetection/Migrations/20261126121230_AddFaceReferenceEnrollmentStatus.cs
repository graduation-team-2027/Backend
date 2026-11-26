using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISmokingDetection.Migrations
{
    /// <inheritdoc />
    public partial class AddFaceReferenceEnrollmentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "FaceReferenceMetadata",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FaceReferenceMetadata",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE [FaceReferenceMetadata] SET [Status] = 3 WHERE [IsActive] = 0;");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "FaceReferenceMetadata");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FaceReferenceMetadata");
        }
    }
}
