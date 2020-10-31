using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class DropApplicationUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropForeignKey(
                 name: "FK_ClassEnrolments_AspNetUsers_ApplicationUserId",
                  table: "ClassEnrolments");

            migrationBuilder.DropIndex(
                    name: "IX_ClassEnrolments_ApplicationUserId",
                    table: "ClassEnrolments");

            migrationBuilder.DropColumn(
                          name: "ApplicationUserId",
                          table: "ClassEnrolments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
