using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class details : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XTasks_XTables_XTableId",
                table: "XTasks");

            migrationBuilder.DropIndex(
                name: "IX_XTasks_XTableId",
                table: "XTasks");

            migrationBuilder.DropColumn(
                name: "XTableId",
                table: "XTasks");

            migrationBuilder.AddColumn<string>(
                name: "TableId",
                table: "XTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_TableId",
                table: "XTasks",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks",
                column: "TableId",
                principalTable: "XTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks");

            migrationBuilder.DropIndex(
                name: "IX_XTasks_TableId",
                table: "XTasks");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "XTasks");

            migrationBuilder.AddColumn<string>(
                name: "XTableId",
                table: "XTasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_XTableId",
                table: "XTasks",
                column: "XTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_XTasks_XTables_XTableId",
                table: "XTasks",
                column: "XTableId",
                principalTable: "XTables",
                principalColumn: "Id");
        }
    }
}
