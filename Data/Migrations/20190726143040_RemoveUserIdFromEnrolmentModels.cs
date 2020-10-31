using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RemoveUserIdFromEnrolmentModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrolments_AspNetUsers_ApplicationUserId1",
                table: "ClassEnrolments");

            migrationBuilder.DropIndex(
                name: "IX_ClassEnrolments_ApplicationUserId1",
                table: "ClassEnrolments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId1",
                table: "ClassEnrolments",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrolments_ApplicationUserId",
                table: "ClassEnrolments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrolments_AspNetUsers_ApplicationUserId",
                table: "ClassEnrolments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrolments_AspNetUsers_ApplicationUserId",
                table: "ClassEnrolments");

            migrationBuilder.DropIndex(
                name: "IX_ClassEnrolments_ApplicationUserId",
                table: "ClassEnrolments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ClassEnrolments",
                newName: "ApplicationUserId1");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId1",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrolments_ApplicationUserId1",
                table: "ClassEnrolments",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrolments_AspNetUsers_ApplicationUserId1",
                table: "ClassEnrolments",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
