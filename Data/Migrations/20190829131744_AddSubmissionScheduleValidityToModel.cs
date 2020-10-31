using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddSubmissionScheduleValidityToModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmissionSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClassId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionSchedules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionValidities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubmissionScheduleId = table.Column<int>(nullable: false),
                    Open = table.Column<DateTime>(nullable: false),
                    Close = table.Column<DateTime>(nullable: false),
                    SummativeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionValidities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionValidities_SubmissionSchedules_SubmissionScheduleId",
                        column: x => x.SubmissionScheduleId,
                        principalTable: "SubmissionSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SubmissionValidities_Summatives_SummativeId",
                        column: x => x.SummativeId,
                        principalTable: "Summatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSchedules_ClassId",
                table: "SubmissionSchedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionValidities_SubmissionScheduleId",
                table: "SubmissionValidities",
                column: "SubmissionScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionValidities_SummativeId",
                table: "SubmissionValidities",
                column: "SummativeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionValidities");

            migrationBuilder.DropTable(
                name: "SubmissionSchedules");
        }
    }
}
