using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Waitsasparenttable : Migration
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

            migrationBuilder.AddForeignKey(
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                column: "FunctionRuntimeInfoFunctionId",
                principalTable: "FunctionInfos",
                principalColumn: "FunctionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits");

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
