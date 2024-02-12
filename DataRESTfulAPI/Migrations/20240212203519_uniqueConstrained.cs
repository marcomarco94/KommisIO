using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataRESTfulAPI.Migrations
{
    /// <inheritdoc />
    public partial class uniqueConstrained : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_PersonnelNumber",
                table: "Employees",
                column: "PersonnelNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Article_ArticleId",
                table: "Article",
                column: "ArticleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_PersonnelNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Article_ArticleId",
                table: "Article");
        }
    }
}
