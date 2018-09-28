using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ECRS_EventManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EventName = table.Column<string>(nullable: true),
                    FormID = table.Column<string>(nullable: true),
                    FormInternalName = table.Column<string>(nullable: true),
                    FormName = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    BillingAddressID = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    Gender = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(maxLength: 255, nullable: false),
                    MiddleInitial = table.Column<string>(maxLength: 2, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 255, nullable: true),
                    NamePrefix = table.Column<string>(maxLength: 31, nullable: true),
                    NameSuffix = table.Column<string>(maxLength: 31, nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Line1 = table.Column<string>(nullable: true),
                    Line2 = table.Column<string>(nullable: true),
                    Line3 = table.Column<string>(nullable: true),
                    PersonID = table.Column<Guid>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Address_Person_PersonID",
                        column: x => x.PersonID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EventID = table.Column<Guid>(nullable: false),
                    FormAdminLink = table.Column<string>(nullable: false),
                    FormEditLink = table.Column<string>(nullable: false),
                    FormEntryID = table.Column<string>(nullable: false),
                    PayorID = table.Column<Guid>(nullable: false),
                    SubmittedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Registration_Event_EventID",
                        column: x => x.EventID,
                        principalTable: "Event",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registration_Person_PayorID",
                        column: x => x.PayorID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationEntry",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    PersonID = table.Column<Guid>(nullable: false),
                    RegistrationID = table.Column<Guid>(nullable: true),
                    SubmittedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationEntry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RegistrationEntry_Person_PersonID",
                        column: x => x.PersonID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationEntry_Registration_RegistrationID",
                        column: x => x.RegistrationID,
                        principalTable: "Registration",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_PersonID",
                table: "Address",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_BillingAddressID",
                table: "Person",
                column: "BillingAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_EventID",
                table: "Registration",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_PayorID",
                table: "Registration",
                column: "PayorID");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationEntry_PersonID",
                table: "RegistrationEntry",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationEntry_RegistrationID",
                table: "RegistrationEntry",
                column: "RegistrationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Address_BillingAddressID",
                table: "Person",
                column: "BillingAddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Person_PersonID",
                table: "Address");

            migrationBuilder.DropTable(
                name: "RegistrationEntry");

            migrationBuilder.DropTable(
                name: "Registration");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
