using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateClassEnrolmentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrolments_Classes_ClassId",
                table: "ClassEnrolments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrolledByUserId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CurrentTopicId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPage",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "ClassEnrolments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "ClassEnrolments",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrolments_Classes_ClassId",
                table: "ClassEnrolments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrolments_Classes_ClassId",
                table: "ClassEnrolments");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "ClassEnrolments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrolledByUserId",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentTopicId",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPage",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "ClassEnrolments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrolments_Classes_ClassId",
                table: "ClassEnrolments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
