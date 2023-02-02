using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFunctionFolderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionInfos",
                table: "FunctionInfos");

            migrationBuilder.RenameTable(
                name: "FunctionInfos",
                newName: "FunctionRuntimeInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionRuntimeInfos",
                table: "FunctionRuntimeInfos",
                column: "FunctionId");

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
                name: "TypeInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventProviderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FunctionFolderId = table.Column<int>(type: "int", nullable: true),
                    FunctionFolderId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypeInfo_FunctionFolders_FunctionFolderId",
                        column: x => x.FunctionFolderId,
                        principalTable: "FunctionFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TypeInfo_FunctionFolders_FunctionFolderId1",
                        column: x => x.FunctionFolderId1,
                        principalTable: "FunctionFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfo_FunctionFolderId",
                table: "TypeInfo",
                column: "FunctionFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeInfo_FunctionFolderId1",
                table: "TypeInfo",
                column: "FunctionFolderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Waits_FunctionRuntimeInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                column: "FunctionRuntimeInfoFunctionId",
                principalTable: "FunctionRuntimeInfos",
                principalColumn: "FunctionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waits_FunctionRuntimeInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits");

            migrationBuilder.DropTable(
                name: "TypeInfo");

            migrationBuilder.DropTable(
                name: "FunctionFolders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionRuntimeInfos",
                table: "FunctionRuntimeInfos");

            migrationBuilder.RenameTable(
                name: "FunctionRuntimeInfos",
                newName: "FunctionInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionInfos",
                table: "FunctionInfos",
                column: "FunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Waits_FunctionInfos_FunctionRuntimeInfoFunctionId",
                table: "Waits",
                column: "FunctionRuntimeInfoFunctionId",
                principalTable: "FunctionInfos",
                principalColumn: "FunctionId");
        }
    }
}
