using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FunctionRuntimeInfos",
                columns: table => new
                {
                    FunctionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatedByClassType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionState = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionRuntimeInfos", x => x.FunctionId);
                });

            migrationBuilder.CreateTable(
                name: "TypeInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FunctionFolderId = table.Column<int>(type: "int", nullable: true),
                    FunctionFolderId1 = table.Column<int>(type: "int", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EventIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    InitiatedByFunctionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateAfterWait = table.Column<int>(type: "int", nullable: false),
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    ParentFunctionWaitId = table.Column<int>(type: "int", nullable: true),
                    IsNode = table.Column<bool>(type: "bit", nullable: false),
                    ReplayType = table.Column<int>(type: "int", nullable: true),
                    WaitType = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentGroupId = table.Column<int>(type: "int", nullable: true),
                    IsOptional = table.Column<bool>(type: "bit", nullable: true),
                    MatchExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetDataExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventProviderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedFunctionDataForMatch = table.Column<bool>(type: "bit", nullable: true),
                    ManyEventsWaitId = table.Column<int>(type: "int", nullable: true),
                    ManyEventsWaitId1 = table.Column<int>(type: "int", nullable: true),
                    ParentFunctionGroupId = table.Column<int>(type: "int", nullable: true),
                    CurrentWaitId = table.Column<int>(type: "int", nullable: true),
                    FunctionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManyFunctionsWaitId = table.Column<int>(type: "int", nullable: true),
                    ManyFunctionsWaitId1 = table.Column<int>(type: "int", nullable: true),
                    WhenCountExpression = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waits_FunctionRuntimeInfos_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "FunctionRuntimeInfos",
                        principalColumn: "FunctionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Waits_Waits_CurrentWaitId",
                        column: x => x.CurrentWaitId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ManyEventsWaitId",
                        column: x => x.ManyEventsWaitId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ManyEventsWaitId1",
                        column: x => x.ManyEventsWaitId1,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ManyFunctionsWaitId",
                        column: x => x.ManyFunctionsWaitId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ManyFunctionsWaitId1",
                        column: x => x.ManyFunctionsWaitId1,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ParentFunctionGroupId",
                        column: x => x.ParentFunctionGroupId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ParentFunctionWaitId",
                        column: x => x.ParentFunctionWaitId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Waits_Waits_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "Waits",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "FunctionFolders",
                columns: new[] { "Id", "LastScanDate", "Path" },
                values: new object[] { 61, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C:\\ResumableFunction\\Example.ProjectApproval\\bin\\Debug\\net7.0" });

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfos_FunctionFolderId",
                table: "TypeInfos",
                column: "FunctionFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfos_FunctionFolderId1",
                table: "TypeInfos",
                column: "FunctionFolderId1");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_CurrentWaitId",
                table: "Waits",
                column: "CurrentWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_FunctionId",
                table: "Waits",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ManyEventsWaitId",
                table: "Waits",
                column: "ManyEventsWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ManyEventsWaitId1",
                table: "Waits",
                column: "ManyEventsWaitId1");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ManyFunctionsWaitId",
                table: "Waits",
                column: "ManyFunctionsWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ManyFunctionsWaitId1",
                table: "Waits",
                column: "ManyFunctionsWaitId1");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ParentFunctionGroupId",
                table: "Waits",
                column: "ParentFunctionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ParentFunctionWaitId",
                table: "Waits",
                column: "ParentFunctionWaitId");

            migrationBuilder.CreateIndex(
                name: "IX_Waits_ParentGroupId",
                table: "Waits",
                column: "ParentGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeInfos");

            migrationBuilder.DropTable(
                name: "Waits");

            migrationBuilder.DropTable(
                name: "FunctionFolders");

            migrationBuilder.DropTable(
                name: "FunctionRuntimeInfos");
        }
    }
}
