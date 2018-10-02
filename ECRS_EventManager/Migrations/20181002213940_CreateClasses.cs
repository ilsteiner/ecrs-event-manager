using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ECRS_EventManager.Migrations
{
    public partial class CreateClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventClass",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EventID = table.Column<Guid>(nullable: false),
                    EventPeriod = table.Column<string>(nullable: false),
                    Leader = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventClass", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventClass_Event_EventID",
                        column: x => x.EventID,
                        principalTable: "Event",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassPriority",
                columns: table => new
                {
                    Priority = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventClassID = table.Column<Guid>(nullable: false),
                    RegistrationEntryID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassPriority", x => x.Priority);
                    table.ForeignKey(
                        name: "FK_ClassPriority_EventClass_EventClassID",
                        column: x => x.EventClassID,
                        principalTable: "EventClass",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassPriority_RegistrationEntry_RegistrationEntryID",
                        column: x => x.RegistrationEntryID,
                        principalTable: "RegistrationEntry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassPriority_EventClassID",
                table: "ClassPriority",
                column: "EventClassID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPriority_RegistrationEntryID",
                table: "ClassPriority",
                column: "RegistrationEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_EventClass_EventID",
                table: "EventClass",
                column: "EventID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassPriority");

            migrationBuilder.DropTable(
                name: "EventClass");
        }
    }
}
