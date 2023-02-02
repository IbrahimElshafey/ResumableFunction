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
                name: "FunctionInfos",
                columns: table => new
                {
                    FunctionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InitiatedByClassType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionInfos", x => x.FunctionId);
                });

            migrationBuilder.CreateTable(
                name: "Wait",
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
                    ReplayType = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsOptional = table.Column<bool>(type: "INTEGER", nullable: true),
                    MatchExpression = table.Column<string>(type: "TEXT", nullable: true),
                    SetDataExpression = table.Column<string>(type: "TEXT", nullable: true),
                    EventProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    EventDataType = table.Column<string>(type: "TEXT", nullable: true),
                    EventData = table.Column<string>(type: "TEXT", nullable: true),
                    NeedFunctionData = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wait", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wait_FunctionInfos_FunctionRuntimeInfoFunctionId",
                        column: x => x.FunctionRuntimeInfoFunctionId,
                        principalTable: "FunctionInfos",
                        principalColumn: "FunctionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wait_FunctionRuntimeInfoFunctionId",
                table: "Wait",
                column: "FunctionRuntimeInfoFunctionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wait");

            migrationBuilder.DropTable(
                name: "FunctionInfos");
        }
    }
}
