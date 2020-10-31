using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class UpdateMultipleChoiceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTopics_CourseUnits_CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.DropIndex(
                name: "IX_CourseTopics_CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.DropColumn(
                name: "CourseUnitId",
                table: "CourseTopics");

            migrationBuilder.AddColumn<string>(
                name: "CourseTopicIds",
                table: "CourseUnits",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MultipleChoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(nullable: true),
                    AnswerA = table.Column<string>(nullable: true),
                    AnswerB = table.Column<string>(nullable: true),
                    AnswerC = table.Column<string>(nullable: true),
                    AnswerD = table.Column<string>(nullable: true),
                    CorrectAnswer = table.Column<string>(nullable: true),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrueFalseQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(nullable: true),
                    CorrectAnswer = table.Column<bool>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrueFalseQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formatives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    CourseTopicId = table.Column<int>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    TrueFalseId = table.Column<int>(nullable: true),
                    MultipleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formatives_MultipleChoice_MultipleId",
                        column: x => x.MultipleId,
                        principalTable: "MultipleChoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Formatives_TrueFalseQuestion_TrueFalseId",
                        column: x => x.TrueFalseId,
                        principalTable: "TrueFalseQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Formatives_MultipleId",
                table: "Formatives",
                column: "MultipleId");

            migrationBuilder.CreateIndex(
                name: "IX_Formatives_TrueFalseId",
                table: "Formatives",
                column: "TrueFalseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Formatives");

            migrationBuilder.DropTable(
                name: "MultipleChoice");

            migrationBuilder.DropTable(
                name: "TrueFalseQuestion");

            migrationBuilder.DropColumn(
                name: "CourseTopicIds",
                table: "CourseUnits");

            migrationBuilder.AddColumn<int>(
                name: "CourseUnitId",
                table: "CourseTopics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopics_CourseUnitId",
                table: "CourseTopics",
                column: "CourseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTopics_CourseUnits_CourseUnitId",
                table: "CourseTopics",
                column: "CourseUnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
