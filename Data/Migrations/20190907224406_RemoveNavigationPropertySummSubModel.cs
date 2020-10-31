using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RemoveNavigationPropertySummSubModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SummativeSubmissions_AspNetUsers_ApplicationUserId1",
                table: "SummativeSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_SummativeSubmissions_ApplicationUserId1",
                table: "SummativeSubmissions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "SummativeSubmissions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "SummativeSubmissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "SummativeSubmissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "SummativeSubmissions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SummativeSubmissions_ApplicationUserId1",
                table: "SummativeSubmissions",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SummativeSubmissions_AspNetUsers_ApplicationUserId1",
                table: "SummativeSubmissions",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
