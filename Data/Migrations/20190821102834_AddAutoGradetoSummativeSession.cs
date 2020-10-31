using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddAutoGradetoSummativeSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SAGradesInOrder",
                table: "SummativeSessions");

            migrationBuilder.AddColumn<int>(
                name: "AutoGrade",
                table: "SummativeSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoGrade",
                table: "SummativeSessions");

            migrationBuilder.AddColumn<string>(
                name: "SAGradesInOrder",
                table: "SummativeSessions",
                nullable: true);
        }
    }
}
