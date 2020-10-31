using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddDirectQuestionAndAssignmentModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_CourseTopics_CourseTopicId",
                table: "Formatives");

            migrationBuilder.DropIndex(
                name: "IX_Formatives_CourseTopicId",
                table: "Formatives");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    SubmissionRequirements = table.Column<string>(nullable: true),
                    AssignmentRequest = table.Column<string>(nullable: true),
                    AssignmentSubmission = table.Column<string>(nullable: true),
                    Open = table.Column<DateTime>(nullable: false),
                    Close = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(nullable: true),
                    AnswerGuide = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Practicals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Requirements = table.Column<string>(nullable: true),
                    PracticalRequest = table.Column<string>(nullable: true),
                    PracticalSubmission = table.Column<string>(nullable: true),
                    Open = table.Column<DateTime>(nullable: false),
                    Close = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Practicals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Summatives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    CourseUnitId = table.Column<int>(nullable: false),
                    MultipleChoiceId = table.Column<int>(nullable: true),
                    TrueFalseId = table.Column<int>(nullable: true),
                    DirectQuestionId = table.Column<int>(nullable: true),
                    AssignmentId = table.Column<int>(nullable: true),
                    PracticalId = table.Column<int>(nullable: true),
                    AssessmentType = table.Column<string>(nullable: true),
                    Weight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Summatives_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Summatives_DirectQuestions_DirectQuestionId",
                        column: x => x.DirectQuestionId,
                        principalTable: "DirectQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Summatives_MultipleChoices_MultipleChoiceId",
                        column: x => x.MultipleChoiceId,
                        principalTable: "MultipleChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Summatives_Practicals_PracticalId",
                        column: x => x.PracticalId,
                        principalTable: "Practicals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Summatives_TrueFalses_TrueFalseId",
                        column: x => x.TrueFalseId,
                        principalTable: "TrueFalses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_AssignmentId",
                table: "Summatives",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_DirectQuestionId",
                table: "Summatives",
                column: "DirectQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_MultipleChoiceId",
                table: "Summatives",
                column: "MultipleChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_PracticalId",
                table: "Summatives",
                column: "PracticalId");

            migrationBuilder.CreateIndex(
                name: "IX_Summatives_TrueFalseId",
                table: "Summatives",
                column: "TrueFalseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Summatives");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "DirectQuestions");

            migrationBuilder.DropTable(
                name: "Practicals");

            migrationBuilder.CreateIndex(
                name: "IX_Formatives_CourseTopicId",
                table: "Formatives",
                column: "CourseTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formatives_CourseTopics_CourseTopicId",
                table: "Formatives",
                column: "CourseTopicId",
                principalTable: "CourseTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
