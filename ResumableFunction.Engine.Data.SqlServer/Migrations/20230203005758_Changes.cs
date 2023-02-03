using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumableFunction.Engine.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NeedFunctionData",
                table: "EventWaits",
                newName: "NeedFunctionDataForMatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NeedFunctionDataForMatch",
                table: "EventWaits",
                newName: "NeedFunctionData");
        }
    }
}
