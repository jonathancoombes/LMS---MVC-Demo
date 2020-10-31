using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateCourseUnitModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CourseUnits");

            migrationBuilder.AddColumn<int>(
                name: "CourseModuleId",
                table: "CourseUnits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CourseUnits",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseModules",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnits_CourseModuleId",
                table: "CourseUnits",
                column: "CourseModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnits_CourseModules_CourseModuleId",
                table: "CourseUnits",
                column: "CourseModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnits_CourseModules_CourseModuleId",
                table: "CourseUnits");

            migrationBuilder.DropIndex(
                name: "IX_CourseUnits_CourseModuleId",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "CourseModuleId",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CourseUnits");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CourseUnits",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseModules",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
