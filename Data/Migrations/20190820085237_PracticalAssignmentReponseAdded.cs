using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class PracticalAssignmentReponseAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignmentGraded",
                table: "SummativeSubmissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PracticalGraded",
                table: "SummativeSubmissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentGraded",
                table: "SummativeSubmissions");

            migrationBuilder.DropColumn(
                name: "PracticalGraded",
                table: "SummativeSubmissions");
        }
    }
}
