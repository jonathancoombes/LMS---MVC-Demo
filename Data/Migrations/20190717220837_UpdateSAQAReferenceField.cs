using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateSAQAReferenceField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NqfLevel",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "SaqaRef",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "SaqaRef",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "SaqaRef",
                table: "CourseUnits",
                newName: "Level");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Formatives",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Formatives",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "CourseUnits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Courses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Formatives");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "CourseUnits",
                newName: "SaqaRef");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Formatives",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "NqfLevel",
                table: "CourseUnits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaqaRef",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaqaRef",
                table: "Courses",
                nullable: true);
        }
    }
}
