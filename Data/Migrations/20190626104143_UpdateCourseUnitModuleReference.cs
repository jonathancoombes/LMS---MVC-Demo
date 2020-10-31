using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateCourseUnitModuleReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseUnitAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UnitId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    CourseModuleId = table.Column<int>(nullable: false),
                    ModuleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUnitAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseUnitAssignments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CourseUnitAssignments_CourseModules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CourseUnitAssignments_CourseUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "CourseUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnitAssignments_CourseId",
                table: "CourseUnitAssignments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnitAssignments_ModuleId",
                table: "CourseUnitAssignments",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnitAssignments_UnitId",
                table: "CourseUnitAssignments",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseUnitAssignments");
        }
    }
}
