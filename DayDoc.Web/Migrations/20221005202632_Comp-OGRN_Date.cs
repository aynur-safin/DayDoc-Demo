using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayDoc.Web.Migrations
{
    public partial class CompOGRN_Date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OGRN_Date",
                table: "Companies",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OGRN_Date",
                table: "Companies");
        }
    }
}
