using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddFASASessionSubmissionModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormativeSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    ClassId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    FAQuestionIdsInOrder = table.Column<string>(nullable: true),
                    FASubmissionInOrder = table.Column<string>(nullable: true),
                    AttemptNumber = table.Column<int>(nullable: false),
                    FAStart = table.Column<DateTime>(nullable: false),
                    FAEnd = table.Column<DateTime>(nullable: true),
                    FAGradesInOrder = table.Column<string>(nullable: true),
                    PercentageAchieved = table.Column<int>(nullable: false),
                    FAFeedback = table.Column<string>(nullable: true),
                    Result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormativeSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SummativeSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    ClassId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    SAIdsInOrder = table.Column<string>(nullable: true),
                    SASubmissionInOrder = table.Column<string>(nullable: true),
                    AttemptNumber = table.Column<int>(nullable: false),
                    SAStart = table.Column<DateTime>(nullable: false),
                    SAEnd = table.Column<DateTime>(nullable: true),
                    SAGradesInOrder = table.Column<string>(nullable: true),
                    PercentageAchieved = table.Column<int>(nullable: false),
                    SAFeedback = table.Column<string>(nullable: true),
                    Result = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummativeSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormativeSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    FormativeId = table.Column<int>(nullable: false),
                    MCAnswer = table.Column<string>(nullable: true),
                    TFAnswer = table.Column<string>(nullable: true),
                    GradePercentage = table.Column<int>(nullable: false),
                    FormativeSessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormativeSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormativeSubmissions_Formatives_FormativeId",
                        column: x => x.FormativeId,
                        principalTable: "Formatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FormativeSubmissions_FormativeSessions_FormativeSessionId",
                        column: x => x.FormativeSessionId,
                        principalTable: "FormativeSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SummativeSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    SummativeId = table.Column<int>(nullable: false),
                    MCAnswer = table.Column<string>(nullable: true),
                    TFAnswer = table.Column<string>(nullable: true),
                    DirectAnswer = table.Column<string>(nullable: true),
                    PracticalSubmission = table.Column<string>(nullable: true),
                    AssignmentSubmission = table.Column<string>(nullable: true),
                    GradePercentage = table.Column<int>(nullable: false),
                    SummativeSessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummativeSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummativeSubmissions_Summatives_SummativeId",
                        column: x => x.SummativeId,
                        principalTable: "Summatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SummativeSubmissions_SummativeSessions_SummativeSessionId",
                        column: x => x.SummativeSessionId,
                        principalTable: "SummativeSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormativeSubmissions_FormativeId",
                table: "FormativeSubmissions",
                column: "FormativeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormativeSubmissions_FormativeSessionId",
                table: "FormativeSubmissions",
                column: "FormativeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SummativeSubmissions_SummativeId",
                table: "SummativeSubmissions",
                column: "SummativeId");

            migrationBuilder.CreateIndex(
                name: "IX_SummativeSubmissions_SummativeSessionId",
                table: "SummativeSubmissions",
                column: "SummativeSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormativeSubmissions");

            migrationBuilder.DropTable(
                name: "SummativeSubmissions");

            migrationBuilder.DropTable(
                name: "FormativeSessions");

            migrationBuilder.DropTable(
                name: "SummativeSessions");
        }
    }
}
