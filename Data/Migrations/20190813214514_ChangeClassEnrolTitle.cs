using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class ChangeClassEnrolTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptNumber",
                table: "FormativeSessions");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "FormativeSessions",
                newName: "ClassEnrolId");

            migrationBuilder.AlterColumn<int>(
                name: "PercentageAchieved",
                table: "FormativeSessions",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClassEnrolId",
                table: "FormativeSessions",
                newName: "ClassId");

            migrationBuilder.AlterColumn<int>(
                name: "PercentageAchieved",
                table: "FormativeSessions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttemptNumber",
                table: "FormativeSessions",
                nullable: false,
                defaultValue: 0);
        }
    }
}
