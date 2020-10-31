using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateSummativeIDPractAssModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summatives_Assignments_AssignmentId1",
                table: "Summatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Summatives_Practicals_PracticalId1",
                table: "Summatives");

            migrationBuilder.DropIndex(
                name: "IX_Summatives_AssignmentId1",
                table: "Summatives");

            migrationBuilder.DropIndex(
                name: "IX_Summatives_PracticalId1",
                table: "Summatives");

            migrationBuilder.DropColumn(
                name: "AssignmentId1",
                table: "Summatives");

            migrationBuilder.DropColumn(
                name: "PracticalId1",
                table: "Summatives");

            migrationBuilder.RenameColumn(
                name: "SummativeId",
                table: "Practicals",
                newName: "SummId");

            migrationBuilder.RenameColumn(
                name: "SummativeId",
                table: "Assignments",
                newName: "SummId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_AssignmentId",
                table: "Summatives",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_PracticalId",
                table: "Summatives",
                column: "PracticalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Summatives_Assignments_AssignmentId",
                table: "Summatives",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Summatives_Practicals_PracticalId",
                table: "Summatives",
                column: "PracticalId",
                principalTable: "Practicals",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summatives_Assignments_AssignmentId",
                table: "Summatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Summatives_Practicals_PracticalId",
                table: "Summatives");

            migrationBuilder.DropIndex(
                name: "IX_Summatives_AssignmentId",
                table: "Summatives");

            migrationBuilder.DropIndex(
                name: "IX_Summatives_PracticalId",
                table: "Summatives");

            migrationBuilder.RenameColumn(
                name: "SummId",
                table: "Practicals",
                newName: "SummativeId");

            migrationBuilder.RenameColumn(
                name: "SummId",
                table: "Assignments",
                newName: "SummativeId");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentId1",
                table: "Summatives",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PracticalId1",
                table: "Summatives",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_AssignmentId1",
                table: "Summatives",
                column: "AssignmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_PracticalId1",
                table: "Summatives",
                column: "PracticalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Summatives_Assignments_AssignmentId1",
                table: "Summatives",
                column: "AssignmentId1",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Summatives_Practicals_PracticalId1",
                table: "Summatives",
                column: "PracticalId1",
                principalTable: "Practicals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
