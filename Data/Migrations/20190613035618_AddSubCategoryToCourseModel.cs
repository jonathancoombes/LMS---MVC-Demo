using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddSubCategoryToCourseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Courses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubCategoryId",
                table: "Courses",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_SubCategories_SubCategoryId",
                table: "Courses",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_SubCategories_SubCategoryId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SubCategoryId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Courses");
        }
    }
}
