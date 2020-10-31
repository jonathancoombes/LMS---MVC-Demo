using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateGradePercentageSummativeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GradePercentage",
                table: "SummativeSubmissions",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GradePercentage",
                table: "SummativeSubmissions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
