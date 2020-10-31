using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateCourseTopicModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LearnerGuidePageRange",
                table: "CourseTopics",
                newName: "PDFPageStart");

            migrationBuilder.RenameColumn(
                name: "LearnerGuide",
                table: "CourseTopics",
                newName: "PDFContent");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CourseUnits",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseUnitId",
                table: "CourseTopics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomContent",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FAOrder",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PDFPageEnd",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopics_CourseUnitId",
                table: "CourseTopics",
                column: "CourseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTopics_CourseUnits_CourseUnitId",
                table: "CourseTopics",
                column: "CourseUnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTopics_CourseUnits_CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.DropIndex(
                name: "IX_CourseTopics_CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "CustomContent",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "FAOrder",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "PDFPageEnd",
                table: "CourseTopics");

            migrationBuilder.RenameColumn(
                name: "PDFPageStart",
                table: "CourseTopics",
                newName: "LearnerGuidePageRange");

            migrationBuilder.RenameColumn(
                name: "PDFContent",
                table: "CourseTopics",
                newName: "LearnerGuide");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CourseUnits",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
