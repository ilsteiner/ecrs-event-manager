using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ECRS_EventManager.Migrations
{
    public partial class RemoveBillingAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_Address_BillingAddressID",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_BillingAddressID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "BillingAddressID",
                table: "Person");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillingAddressID",
                table: "Person",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Person_BillingAddressID",
                table: "Person",
                column: "BillingAddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Address_BillingAddressID",
                table: "Person",
                column: "BillingAddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
