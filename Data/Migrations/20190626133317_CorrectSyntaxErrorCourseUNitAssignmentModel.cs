using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class CorrectSyntaxErrorCourseUNitAssignmentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnitAssignments_CourseModules_ModuleId",
                table: "CourseUnitAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CourseUnitAssignments_ModuleId",
                table: "CourseUnitAssignments");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "CourseUnitAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnitAssignments_CourseModuleId",
                table: "CourseUnitAssignments",
                column: "CourseModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnitAssignments_CourseModules_CourseModuleId",
                table: "CourseUnitAssignments",
                column: "CourseModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnitAssignments_CourseModules_CourseModuleId",
                table: "CourseUnitAssignments");

            migrationBuilder.DropIndex(
                name: "IX_CourseUnitAssignments_CourseModuleId",
                table: "CourseUnitAssignments");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "CourseUnitAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnitAssignments_ModuleId",
                table: "CourseUnitAssignments",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnitAssignments_CourseModules_ModuleId",
                table: "CourseUnitAssignments",
                column: "ModuleId",
                principalTable: "CourseModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
