using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ECRS_EventManager.Migrations
{
    public partial class AddConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FormID",
                table: "Event",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Event_FormID",
                table: "Event",
                column: "FormID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Event_FormID",
                table: "Event");

            migrationBuilder.AlterColumn<string>(
                name: "FormID",
                table: "Event",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
