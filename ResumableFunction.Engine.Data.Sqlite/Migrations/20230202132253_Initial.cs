using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FunctionFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FunctionRuntimeInfos",
                columns: table => new
                {
                    FunctionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InitiatedByClassType = table.Column<string>(type: "TEXT", nullable: true),
                    FunctionState = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionRuntimeInfos", x => x.FunctionId);
                });

            migrationBuilder.CreateTable(
                name: "TypeInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    FunctionFolderId = table.Column<int>(type: "INTEGER", nullable: true),
                    FunctionFolderId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypeInfos_FunctionFolders_FunctionFolderId",
                        column: x => x.FunctionFolderId,
                        principalTable: "FunctionFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TypeInfos_FunctionFolders_FunctionFolderId1",
                        column: x => x.FunctionFolderId1,
                        principalTable: "FunctionFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Waits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    EventIdentifier = table.Column<string>(type: "TEXT", nullable: true),
                    IsFirst = table.Column<bool>(type: "INTEGER", nullable: false),
                    InitiatedByFunctionName = table.Column<string>(type: "TEXT", nullable: true),
                    StateAfterWait = table.Column<int>(type: "INTEGER", nullable: false),
                    FunctionRuntimeInfoFunctionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ParentFunctionWaitId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsNode = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReplayType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waits_FunctionRuntimeInfos_FunctionRuntimeInfoFunctionId",
                        column: x => x.FunctionRuntimeInfoFunctionId,
                        principalTable: "FunctionRuntimeInfos",
                        principalColumn: "FunctionId");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfos_FunctionFolderId",
                table: "TypeInfos",
                column: "FunctionFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfos_FunctionFolderId1",
                table: "TypeInfos",
                column: "FunctionFolderId1");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                column: "FunctionRuntimeInfoFunctionId");

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
                name: "FK_AllEventsWaits_Waits_Id",
                table: "AllEventsWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AllFunctionsWaits_Waits_Id",
                table: "AllFunctionsWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyEventWaits_Waits_Id",
                table: "AnyEventWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyFunctionWaits_Waits_Id",
                table: "AnyFunctionWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_EventWaits_Waits_Id",
                table: "EventWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_FunctionWaits_Waits_CurrentWaitId",
                table: "FunctionWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_FunctionWaits_Waits_Id",
                table: "FunctionWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyEventWaits_EventWaits_MatchedEventId",
                table: "AnyEventWaits");

            migrationBuilder.DropForeignKey(
                name: "FK_AnyFunctionWaits_FunctionWaits_MatchedFunctionId",
                table: "AnyFunctionWaits");

            migrationBuilder.DropTable(
                name: "TypeInfos");

            migrationBuilder.DropTable(
                name: "FunctionFolders");

            migrationBuilder.DropTable(
                name: "Waits");

            migrationBuilder.DropTable(
                name: "FunctionRuntimeInfos");

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
        }
    }
}
