using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class histfix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_History_XHistoryId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_XHistoryId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "XHistoryId",
                table: "Contents");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "History",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "XContentXHistory",
                columns: table => new
                {
                    ContentsId = table.Column<string>(type: "text", nullable: false),
                    XHistoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XContentXHistory", x => new { x.ContentsId, x.XHistoryId });
                    table.ForeignKey(
                        name: "FK_XContentXHistory_Contents_ContentsId",
                        column: x => x.ContentsId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XContentXHistory_History_XHistoryId",
                        column: x => x.XHistoryId,
                        principalTable: "History",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XContentXHistory_XHistoryId",
                table: "XContentXHistory",
                column: "XHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XContentXHistory");

            migrationBuilder.AlterColumn<decimal>(
                name: "Id",
                table: "History",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "XHistoryId",
                table: "Contents",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_XHistoryId",
                table: "Contents",
                column: "XHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_History_XHistoryId",
                table: "Contents",
                column: "XHistoryId",
                principalTable: "History",
                principalColumn: "Id");
        }
    }
}
