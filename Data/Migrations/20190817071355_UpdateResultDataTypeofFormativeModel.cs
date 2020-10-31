using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateResultDataTypeofFormativeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GradePercentage",
                table: "FormativeSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "FormativeSubmissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "FormativeSubmissions");

            migrationBuilder.AddColumn<int>(
                name: "GradePercentage",
                table: "FormativeSubmissions",
                nullable: false,
                defaultValue: 0);
        }
    }
}
