using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class SeedDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "Learners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactNumber",
                table: "Learners",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Learners",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityNumber",
                table: "Learners",
                nullable: true);
        }
    }
}
