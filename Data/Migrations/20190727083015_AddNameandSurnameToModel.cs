using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddNameandSurnameToModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
               name: "UserSurname",
               table: "ClassEnrolments",
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ClassEnrolments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ClassEnrolments");

            migrationBuilder.DropColumn(
               name: "UserSurname",
               table: "ClassEnrolments");

          
        }
    }
}
