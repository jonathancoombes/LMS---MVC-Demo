using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddFormativeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_MultipleChoice_MultipleId",
                table: "Formatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_TrueFalseQuestion_TrueFalseId",
                table: "Formatives");

            migrationBuilder.DropTable(
                name: "TrueFalseQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MultipleChoice",
                table: "MultipleChoice");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "MultipleChoice");

            migrationBuilder.RenameTable(
                name: "MultipleChoice",
                newName: "MultipleChoices");

            migrationBuilder.RenameColumn(
                name: "MultipleId",
                table: "Formatives",
                newName: "MultipleChoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Formatives_MultipleId",
                table: "Formatives",
                newName: "IX_Formatives_MultipleChoiceId");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionType",
                table: "Formatives",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultipleChoices",
                table: "MultipleChoices",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TrueFalses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(nullable: true),
                    CorrectAnswer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrueFalses", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Formatives_MultipleChoices_MultipleChoiceId",
                table: "Formatives",
                column: "MultipleChoiceId",
                principalTable: "MultipleChoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Formatives_TrueFalses_TrueFalseId",
                table: "Formatives",
                column: "TrueFalseId",
                principalTable: "TrueFalses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_CourseTopics_CourseTopicId",
                table: "Formatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_MultipleChoices_MultipleChoiceId",
                table: "Formatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Formatives_TrueFalses_TrueFalseId",
                table: "Formatives");

            migrationBuilder.DropTable(
                name: "TrueFalses");

            migrationBuilder.DropIndex(
                name: "IX_Formatives_CourseTopicId",
                table: "Formatives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MultipleChoices",
                table: "MultipleChoices");

            migrationBuilder.RenameTable(
                name: "MultipleChoices",
                newName: "MultipleChoice");

            migrationBuilder.RenameColumn(
                name: "MultipleChoiceId",
                table: "Formatives",
                newName: "MultipleId");

            migrationBuilder.RenameIndex(
                name: "IX_Formatives_MultipleChoiceId",
                table: "Formatives",
                newName: "IX_Formatives_MultipleId");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionType",
                table: "Formatives",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "MultipleChoice",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultipleChoice",
                table: "MultipleChoice",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TrueFalseQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorrectAnswer = table.Column<bool>(nullable: false),
                    Question = table.Column<string>(nullable: true),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrueFalseQuestion", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Formatives_MultipleChoice_MultipleId",
                table: "Formatives",
                column: "MultipleId",
                principalTable: "MultipleChoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Formatives_TrueFalseQuestion_TrueFalseId",
                table: "Formatives",
                column: "TrueFalseId",
                principalTable: "TrueFalseQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
