using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Data.Migrations
{
    public partial class AddSupportRequestResponseModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestHeading = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    Open = table.Column<DateTime>(nullable: false),
                    Closed = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ClassId = table.Column<int>(nullable: false),
                    ResponseIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportResponse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SupportRequestId = table.Column<int>(nullable: false),
                    ResponseDate = table.Column<DateTime>(nullable: false),
                    ResponseBody = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportResponse_SupportRequests_SupportRequestId",
                        column: x => x.SupportRequestId,
                        principalTable: "SupportRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportResponse_SupportRequestId",
                table: "SupportResponse",
                column: "SupportRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportResponse");

            migrationBuilder.DropTable(
                name: "SupportRequests");
        }
    }
}
