using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayDoc.Web.Migrations
{
    public partial class CompCompType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompType",
                table: "Companies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompType",
                table: "Companies");
        }
    }
}
