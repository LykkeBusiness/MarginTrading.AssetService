using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddRelationBetweenProductAndMarketSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Market",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "MarketId",
                schema: "dbo",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_MarketId",
                schema: "dbo",
                table: "Products",
                column: "MarketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products",
                column: "MarketId",
                principalSchema: "dbo",
                principalTable: "MarketSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MarketId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MarketId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Market",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
