using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RemovePDFStartEndFromTopicModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDFPageEnd",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "PDFPageStart",
                table: "CourseTopics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PDFPageEnd",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PDFPageStart",
                table: "CourseTopics",
                nullable: true);
        }
    }
}
