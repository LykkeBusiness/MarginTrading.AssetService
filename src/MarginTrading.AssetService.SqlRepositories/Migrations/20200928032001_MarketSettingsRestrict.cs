using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class MarketSettingsRestrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products",
                column: "MarketId",
                principalSchema: "dbo",
                principalTable: "MarketSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
