using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class cont : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_XTasks_XTaskId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_XTaskId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "XTaskId",
                table: "Contents");

            migrationBuilder.CreateTable(
                name: "XContentXTask",
                columns: table => new
                {
                    ContentsId = table.Column<string>(type: "text", nullable: false),
                    XTaskId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XContentXTask", x => new { x.ContentsId, x.XTaskId });
                    table.ForeignKey(
                        name: "FK_XContentXTask_Contents_ContentsId",
                        column: x => x.ContentsId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XContentXTask_XTasks_XTaskId",
                        column: x => x.XTaskId,
                        principalTable: "XTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XContentXTask_XTaskId",
                table: "XContentXTask",
                column: "XTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XContentXTask");

            migrationBuilder.AddColumn<string>(
                name: "XTaskId",
                table: "Contents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_XTaskId",
                table: "Contents",
                column: "XTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_XTasks_XTaskId",
                table: "Contents",
                column: "XTaskId",
                principalTable: "XTasks",
                principalColumn: "Id");
        }
    }
}
