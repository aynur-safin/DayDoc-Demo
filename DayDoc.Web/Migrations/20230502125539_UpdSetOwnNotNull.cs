using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayDoc.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdSetOwnNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_Companies_OwnCompanyId",
                table: "Settings");

            migrationBuilder.AlterColumn<int>(
                name: "OwnCompanyId",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_Companies_OwnCompanyId",
                table: "Settings",
                column: "OwnCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_Companies_OwnCompanyId",
                table: "Settings");

            migrationBuilder.AlterColumn<int>(
                name: "OwnCompanyId",
                table: "Settings",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_Companies_OwnCompanyId",
                table: "Settings",
                column: "OwnCompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
