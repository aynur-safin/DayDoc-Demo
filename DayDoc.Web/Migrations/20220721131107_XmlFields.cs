using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayDoc.Web.Migrations
{
    public partial class XmlFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkName",
                table: "Settings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Dogovor_Date",
                table: "Docs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Dogovor_Num",
                table: "Docs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkName",
                table: "Docs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdoCompanyId",
                table: "Companies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OGRN",
                table: "Companies",
                type: "TEXT",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signatory_Basis",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signatory_FirstName",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signatory_LastName",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signatory_MiddleName",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signatory_Position",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_EdoCompanyId",
                table: "Companies",
                column: "EdoCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Companies_EdoCompanyId",
                table: "Companies",
                column: "EdoCompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Companies_EdoCompanyId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_EdoCompanyId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "WorkName",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Dogovor_Date",
                table: "Docs");

            migrationBuilder.DropColumn(
                name: "Dogovor_Num",
                table: "Docs");

            migrationBuilder.DropColumn(
                name: "WorkName",
                table: "Docs");

            migrationBuilder.DropColumn(
                name: "EdoCompanyId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OGRN",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Signatory_Basis",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Signatory_FirstName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Signatory_LastName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Signatory_MiddleName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Signatory_Position",
                table: "Companies");
        }
    }
}
