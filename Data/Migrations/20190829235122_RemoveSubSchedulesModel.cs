using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RemoveSubSchedulesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionValidities_SubmissionSchedules_SubmissionScheduleId",
                table: "SubmissionValidities");

            migrationBuilder.DropTable(
                name: "SubmissionSchedules");

            migrationBuilder.RenameColumn(
                name: "SubmissionScheduleId",
                table: "SubmissionValidities",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionValidities_SubmissionScheduleId",
                table: "SubmissionValidities",
                newName: "IX_SubmissionValidities_ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionValidities_Classes_ClassId",
                table: "SubmissionValidities",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionValidities_Classes_ClassId",
                table: "SubmissionValidities");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "SubmissionValidities",
                newName: "SubmissionScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionValidities_ClassId",
                table: "SubmissionValidities",
                newName: "IX_SubmissionValidities_SubmissionScheduleId");

            migrationBuilder.CreateTable(
                name: "SubmissionSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClassId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionSchedules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSchedules_ClassId",
                table: "SubmissionSchedules",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionValidities_SubmissionSchedules_SubmissionScheduleId",
                table: "SubmissionValidities",
                column: "SubmissionScheduleId",
                principalTable: "SubmissionSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
