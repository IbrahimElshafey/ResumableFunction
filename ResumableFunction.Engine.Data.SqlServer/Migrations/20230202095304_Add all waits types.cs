using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Addallwaitstypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnyEventWaitId",
                table: "EventWaits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AllFunctionsWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllFunctionsWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllFunctionsWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnyEventWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchedEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnyEventWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnyEventWaits_EventWaits_MatchedEventId",
                        column: x => x.MatchedEventId,
                        principalTable: "EventWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnyEventWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnyFunctionWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchedFunctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnyFunctionWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnyFunctionWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunctionWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentFunctionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentWaitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FunctionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllFunctionsWaitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllFunctionsWaitId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AnyFunctionWaitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FunctionWaits_AllFunctionsWaits_AllFunctionsWaitId",
                        column: x => x.AllFunctionsWaitId,
                        principalTable: "AllFunctionsWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FunctionWaits_AllFunctionsWaits_AllFunctionsWaitId1",
                        column: x => x.AllFunctionsWaitId1,
                        principalTable: "AllFunctionsWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FunctionWaits_AnyFunctionWaits_AnyFunctionWaitId",
                        column: x => x.AnyFunctionWaitId,
                        principalTable: "AnyFunctionWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FunctionWaits_Waits_CurrentWaitId",
                        column: x => x.CurrentWaitId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FunctionWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AnyEventWaitId",
                table: "EventWaits",
                column: "AnyEventWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_AnyEventWaits_MatchedEventId",
                table: "AnyEventWaits",
                column: "MatchedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AnyFunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits",
                column: "MatchedFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionWaits_AllFunctionsWaitId",
                table: "FunctionWaits",
                column: "AllFunctionsWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionWaits_AllFunctionsWaitId1",
                table: "FunctionWaits",
                column: "AllFunctionsWaitId1");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionWaits_AnyFunctionWaitId",
                table: "FunctionWaits",
                column: "AnyFunctionWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionWaits_CurrentWaitId",
                table: "FunctionWaits",
                column: "CurrentWaitId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventWaits_AnyEventWaits_AnyEventWaitId",
                table: "EventWaits",
                column: "AnyEventWaitId",
                principalTable: "AnyEventWaits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnyFunctionWaits_FunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits",
                column: "MatchedFunctionId",
                principalTable: "FunctionWaits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventWaits_AnyEventWaits_AnyEventWaitId",
                table: "EventWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyFunctionWaits_FunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits");

            migrationBuilder.DropTable(
                name: "AnyEventWaits");

            migrationBuilder.DropTable(
                name: "FunctionWaits");

            migrationBuilder.DropTable(
                name: "AllFunctionsWaits");

            migrationBuilder.DropTable(
                name: "AnyFunctionWaits");

            migrationBuilder.DropIndex(
                name: "IX_EventWaits_AnyEventWaitId",
                table: "EventWaits");

            migrationBuilder.DropColumn(
                name: "AnyEventWaitId",
                table: "EventWaits");
        }
    }
}
