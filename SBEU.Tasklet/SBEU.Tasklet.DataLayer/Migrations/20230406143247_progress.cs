using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "XTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "History");

            migrationBuilder.AlterColumn<string>(
                name: "TableId",
                table: "XTasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "XTasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "History",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskProgress",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskProgress", x => x.Id);
                });

            //migrationBuilder.Sql("Insert Into TaskProgress Calues (0,'New')");

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_StatusId",
                table: "XTasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_History_StatusId",
                table: "History",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_TaskProgress_StatusId",
                table: "History",
                column: "StatusId",
                principalTable: "TaskProgress",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_XTasks_TaskProgress_StatusId",
                table: "XTasks",
                column: "StatusId",
                principalTable: "TaskProgress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks",
                column: "TableId",
                principalTable: "XTables",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_TaskProgress_StatusId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_XTasks_TaskProgress_StatusId",
                table: "XTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks");

            migrationBuilder.DropTable(
                name: "TaskProgress");

            migrationBuilder.DropIndex(
                name: "IX_XTasks_StatusId",
                table: "XTasks");

            migrationBuilder.DropIndex(
                name: "IX_History_StatusId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "XTasks");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "History");

            migrationBuilder.AlterColumn<string>(
                name: "TableId",
                table: "XTasks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "XTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "History",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_XTasks_XTables_TableId",
                table: "XTasks",
                column: "TableId",
                principalTable: "XTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
