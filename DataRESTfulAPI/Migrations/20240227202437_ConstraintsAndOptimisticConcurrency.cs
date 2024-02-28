using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataRESTfulAPI.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintsAndOptimisticConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "StockPositions",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "PickingOrders",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "PickingOrderPositions",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "StockPositions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PickingOrders");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PickingOrderPositions");
        }
    }
}
