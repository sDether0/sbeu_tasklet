using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class fewers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "XTables",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XIdentityUserXTable",
                columns: table => new
                {
                    TablesId = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XIdentityUserXTable", x => new { x.TablesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_XIdentityUserXTable_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XIdentityUserXTable_XTables_TablesId",
                        column: x => x.TablesId,
                        principalTable: "XTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    ExecutorId = table.Column<string>(type: "text", nullable: false),
                    XTableId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XTasks_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XTasks_AspNetUsers_ExecutorId",
                        column: x => x.ExecutorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XTasks_XTables_XTableId",
                        column: x => x.XTableId,
                        principalTable: "XTables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_XIdentityUserXTable_UsersId",
                table: "XIdentityUserXTable",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_AuthorId",
                table: "XTasks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_ExecutorId",
                table: "XTasks",
                column: "ExecutorId");

            migrationBuilder.CreateIndex(
                name: "IX_XTasks_XTableId",
                table: "XTasks",
                column: "XTableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XIdentityUserXTable");

            migrationBuilder.DropTable(
                name: "XTasks");

            migrationBuilder.DropTable(
                name: "XTables");
        }
    }
}
