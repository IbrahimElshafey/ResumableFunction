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
                name: "FunctionInfos",
                columns: table => new
                {
                    FunctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitiatedByClassType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionInfos", x => x.FunctionId);
                });

            migrationBuilder.CreateTable(
                name: "Wait",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EventIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    InitiatedByFunctionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateAfterWait = table.Column<int>(type: "int", nullable: false),
                    FunctionRuntimeInfoFunctionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentFunctionWaitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsNode = table.Column<bool>(type: "bit", nullable: false),
                    ReplayType = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOptional = table.Column<bool>(type: "bit", nullable: true),
                    MatchExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetDataExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventProviderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedFunctionData = table.Column<bool>(type: "bit", nullable: true)
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
