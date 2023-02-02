using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Waitsasparenttable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "EventData",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "EventDataType",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "EventProviderName",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "MatchExpression",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "NeedFunctionData",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "ParentGroupId",
                table: "Waits");

            migrationBuilder.DropColumn(
                name: "SetDataExpression",
                table: "Waits");

            migrationBuilder.CreateTable(
                name: "AllEventsWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WhenCountExpression = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllEventsWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllEventsWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    MatchExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetDataExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventProviderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedFunctionData = table.Column<bool>(type: "bit", nullable: false),
                    AllEventsWaitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllEventsWaitId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventWaits_AllEventsWaits_AllEventsWaitId",
                        column: x => x.AllEventsWaitId,
                        principalTable: "AllEventsWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventWaits_AllEventsWaits_AllEventsWaitId1",
                        column: x => x.AllEventsWaitId1,
                        principalTable: "AllEventsWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AllEventsWaitId",
                table: "EventWaits",
                column: "AllEventsWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AllEventsWaitId1",
                table: "EventWaits",
                column: "AllEventsWaitId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventWaits");

            migrationBuilder.DropTable(
                name: "AllEventsWaits");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventData",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventDataType",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventProviderName",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "Waits",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchExpression",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeedFunctionData",
                table: "Waits",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentGroupId",
                table: "Waits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetDataExpression",
                table: "Waits",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
