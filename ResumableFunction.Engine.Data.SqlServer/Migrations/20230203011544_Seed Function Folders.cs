using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SeedFunctionFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FunctionFolders",
                columns: new[] { "Id", "LastScanDate", "Path" },
                values: new object[] { 97, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C:\\Users\\Ibrahim\\source\\repos\\WorkflowInCode.ConsoleTest\\Example.ProjectApproval\\bin\\Debug\\net7.0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FunctionFolders",
                keyColumn: "Id",
                keyValue: 97);
        }
    }
}
