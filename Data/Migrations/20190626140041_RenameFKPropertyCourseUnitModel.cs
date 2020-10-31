using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RenameFKPropertyCourseUnitModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnitAssignments_CourseUnits_UnitId",
                table: "CourseUnitAssignments");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "CourseUnitAssignments",
                newName: "CourseUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseUnitAssignments_UnitId",
                table: "CourseUnitAssignments",
                newName: "IX_CourseUnitAssignments_CourseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnitAssignments_CourseUnits_CourseUnitId",
                table: "CourseUnitAssignments",
                column: "CourseUnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnitAssignments_CourseUnits_CourseUnitId",
                table: "CourseUnitAssignments");

            migrationBuilder.RenameColumn(
                name: "CourseUnitId",
                table: "CourseUnitAssignments",
                newName: "UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseUnitAssignments_CourseUnitId",
                table: "CourseUnitAssignments",
                newName: "IX_CourseUnitAssignments_UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnitAssignments_CourseUnits_UnitId",
                table: "CourseUnitAssignments",
                column: "UnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
