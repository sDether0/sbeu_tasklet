using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBEU.Tasklet.DataLayer.Migrations
{
    public partial class switchpush : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPushOn",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPushOn",
                table: "AspNetUsers");
        }
    }
}
