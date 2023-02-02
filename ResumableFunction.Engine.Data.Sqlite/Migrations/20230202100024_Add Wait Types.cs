using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddWaitTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wait_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Wait");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wait",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "EventData",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "EventDataType",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "EventProviderName",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "MatchExpression",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "NeedFunctionData",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "ParentGroupId",
                table: "Wait");

            migrationBuilder.DropColumn(
                name: "SetDataExpression",
                table: "Wait");

            migrationBuilder.RenameTable(
                name: "Wait",
                newName: "Waits");

            migrationBuilder.RenameIndex(
                name: "IX_Wait_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                newName: "IX_Waits_FunctionRuntimeInfoFunctionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Waits",
                table: "Waits",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AllEventsWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WhenCountExpression = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "AllFunctionsWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchedEventId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnyEventWaits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnyEventWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsOptional = table.Column<bool>(type: "INTEGER", nullable: false),
                    MatchExpression = table.Column<string>(type: "TEXT", nullable: true),
                    SetDataExpression = table.Column<string>(type: "TEXT", nullable: true),
                    EventProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    EventDataType = table.Column<string>(type: "TEXT", nullable: true),
                    EventData = table.Column<string>(type: "TEXT", nullable: true),
                    NeedFunctionData = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllEventsWaitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AllEventsWaitId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    AnyEventWaitId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                        name: "FK_EventWaits_AnyEventWaits_AnyEventWaitId",
                        column: x => x.AnyEventWaitId,
                        principalTable: "AnyEventWaits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventWaits_Waits_Id",
                        column: x => x.Id,
                        principalTable: "Waits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnyFunctionWaits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchedFunctionId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentFunctionGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CurrentWaitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FunctionName = table.Column<string>(type: "TEXT", nullable: true),
                    AllFunctionsWaitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AllFunctionsWaitId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    AnyFunctionWaitId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                name: "IX_AnyEventWaits_MatchedEventId",
                table: "AnyEventWaits",
                column: "MatchedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AnyFunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits",
                column: "MatchedFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AllEventsWaitId",
                table: "EventWaits",
                column: "AllEventsWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AllEventsWaitId1",
                table: "EventWaits",
                column: "AllEventsWaitId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventWaits_AnyEventWaitId",
                table: "EventWaits",
                column: "AnyEventWaitId");

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
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                column: "FunctionRuntimeInfoFunctionId",
                principalTable: "FunctionInfos",
                principalColumn: "FunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnyEventWaits_EventWaits_MatchedEventId",
                table: "AnyEventWaits",
                column: "MatchedEventId",
                principalTable: "EventWaits",
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
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyEventWaits_EventWaits_MatchedEventId",
                table: "AnyEventWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyFunctionWaits_FunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits");

            migrationBuilder.DropTable(
                name: "EventWaits");

            migrationBuilder.DropTable(
                name: "AllEventsWaits");

            migrationBuilder.DropTable(
                name: "AnyEventWaits");

            migrationBuilder.DropTable(
                name: "FunctionWaits");

            migrationBuilder.DropTable(
                name: "AllFunctionsWaits");

            migrationBuilder.DropTable(
                name: "AnyFunctionWaits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Waits",
                table: "Waits");

            migrationBuilder.RenameTable(
                name: "Waits",
                newName: "Wait");

            migrationBuilder.RenameIndex(
                name: "IX_Waits_FunctionRuntimeInfoFunctionId",
                table: "Wait",
                newName: "IX_Wait_FunctionRuntimeInfoFunctionId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Wait",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventData",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventDataType",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventProviderName",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "Wait",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchExpression",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeedFunctionData",
                table: "Wait",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentGroupId",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetDataExpression",
                table: "Wait",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wait",
                table: "Wait",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wait_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Wait",
                column: "FunctionRuntimeInfoFunctionId",
                principalTable: "FunctionInfos",
                principalColumn: "FunctionId");
        }
    }
}
