using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class notekeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Note_TaskId",
                table: "Note",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_UserId",
                table: "Note",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_AspNetUsers_UserId",
                table: "Note",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_XTasks_TaskId",
                table: "Note",
                column: "TaskId",
                principalTable: "XTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_AspNetUsers_UserId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_XTasks_TaskId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_TaskId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_UserId",
                table: "Note");
        }
    }
}
