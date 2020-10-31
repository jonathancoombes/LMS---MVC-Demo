using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class RemoveDateFromPractAssModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Close",
                table: "Practicals");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "Practicals");

            migrationBuilder.DropColumn(
                name: "Close",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "Assignments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Close",
                table: "Practicals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Open",
                table: "Practicals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Close",
                table: "Assignments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Open",
                table: "Assignments",
                nullable: true);
        }
    }
}
