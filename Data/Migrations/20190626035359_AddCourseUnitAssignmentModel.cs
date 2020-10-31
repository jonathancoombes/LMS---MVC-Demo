using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddCourseUnitAssignmentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseModuleId",
                table: "CourseUnits",
                nullable: false,
                defaultValue: 0);

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
