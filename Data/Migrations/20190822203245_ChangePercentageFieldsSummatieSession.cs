using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class ChangePercentageFieldsSummatieSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Result",
                table: "SummativeSessions",
                newName: "FinalResult");

            migrationBuilder.RenameColumn(
                name: "PercentageAchieved",
                table: "SummativeSessions",
                newName: "FinalPercentageAchieved");

            migrationBuilder.RenameColumn(
                name: "AutoGrade",
                table: "SummativeSessions",
                newName: "QuestionInProgressGrade");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentGradeTotal",
                table: "SummativeSessions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PracticalGradeTotal",
                table: "SummativeSessions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionGradeTotal",
                table: "SummativeSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentGradeTotal",
                table: "SummativeSessions");

            migrationBuilder.DropColumn(
                name: "PracticalGradeTotal",
                table: "SummativeSessions");

            migrationBuilder.DropColumn(
                name: "QuestionGradeTotal",
                table: "SummativeSessions");

            migrationBuilder.RenameColumn(
                name: "QuestionInProgressGrade",
                table: "SummativeSessions",
                newName: "AutoGrade");

            migrationBuilder.RenameColumn(
                name: "FinalResult",
                table: "SummativeSessions",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "FinalPercentageAchieved",
                table: "SummativeSessions",
                newName: "PercentageAchieved");
        }
    }
}
