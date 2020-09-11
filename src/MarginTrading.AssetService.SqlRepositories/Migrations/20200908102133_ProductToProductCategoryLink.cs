using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class ProductToProductCategoryLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "dbo",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                schema: "dbo",
                table: "Products",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
