using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class tasklink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Links",
                table: "XTasks",
                type: "text[]",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chats",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Links",
                table: "XTasks");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
