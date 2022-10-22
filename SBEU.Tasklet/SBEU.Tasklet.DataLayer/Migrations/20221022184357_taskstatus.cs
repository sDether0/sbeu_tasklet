using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class taskstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "XTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "XTasks");
        }
    }
}
