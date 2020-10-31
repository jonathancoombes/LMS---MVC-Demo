using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateCourseUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PracticalSubmission",
                table: "Practicals");

            migrationBuilder.DropColumn(
                name: "AssignmentSubmission",
                table: "Assignments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PracticalSubmission",
                table: "Practicals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignmentSubmission",
                table: "Assignments",
                nullable: true);
        }
    }
}
