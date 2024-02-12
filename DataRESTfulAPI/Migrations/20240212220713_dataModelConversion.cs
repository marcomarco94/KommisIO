using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataRESTfulAPI.Migrations
{
    /// <inheritdoc />
    public partial class dataModelConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageReportEntities_Article_ArticleId",
                table: "DamageReportEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageReportEntities_Employees_EmployeeId",
                table: "DamageReportEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_PickingOrderPositions_Article_ArticleId",
                table: "PickingOrderPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPositions_Article_ArticleEntityId",
                table: "StockPositions");

            migrationBuilder.RenameColumn(
                name: "ArticleEntityId",
                table: "StockPositions",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_StockPositions_ArticleEntityId",
                table: "StockPositions",
                newName: "IX_StockPositions_ArticleId");

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "PickingOrderPositions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "DamageReportEntities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "DamageReportEntities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReportEntities_Article_ArticleId",
                table: "DamageReportEntities",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReportEntities_Employees_EmployeeId",
                table: "DamageReportEntities",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PickingOrderPositions_Article_ArticleId",
                table: "PickingOrderPositions",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockPositions_Article_ArticleId",
                table: "StockPositions",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageReportEntities_Article_ArticleId",
                table: "DamageReportEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageReportEntities_Employees_EmployeeId",
                table: "DamageReportEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_PickingOrderPositions_Article_ArticleId",
                table: "PickingOrderPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPositions_Article_ArticleId",
                table: "StockPositions");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "StockPositions",
                newName: "ArticleEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_StockPositions_ArticleId",
                table: "StockPositions",
                newName: "IX_StockPositions_ArticleEntityId");

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "PickingOrderPositions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "DamageReportEntities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "DamageReportEntities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReportEntities_Article_ArticleId",
                table: "DamageReportEntities",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReportEntities_Employees_EmployeeId",
                table: "DamageReportEntities",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PickingOrderPositions_Article_ArticleId",
                table: "PickingOrderPositions",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPositions_Article_ArticleEntityId",
                table: "StockPositions",
                column: "ArticleEntityId",
                principalTable: "Article",
                principalColumn: "Id");
        }
    }
}
