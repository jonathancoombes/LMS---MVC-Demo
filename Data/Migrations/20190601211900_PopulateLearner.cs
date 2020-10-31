using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class PopulateLearner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql("INSERT INTO Learners (Name, Surname) VALUES('Jonathan', 'Coombes')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
