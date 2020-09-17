using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class ProductLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AssetType",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TickFormula",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "MarketId",
                schema: "dbo",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetTypeId",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TickFormulaId",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AssetTypeId",
                schema: "dbo",
                table: "Products",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TickFormulaId",
                schema: "dbo",
                table: "Products",
                column: "TickFormulaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AssetTypes_AssetTypeId",
                schema: "dbo",
                table: "Products",
                column: "AssetTypeId",
                principalSchema: "dbo",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products",
                column: "MarketId",
                principalSchema: "dbo",
                principalTable: "MarketSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TickFormulas_TickFormulaId",
                schema: "dbo",
                table: "Products",
                column: "TickFormulaId",
                principalSchema: "dbo",
                principalTable: "TickFormulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AssetTypes_AssetTypeId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketSettings_MarketId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_TickFormulas_TickFormulaId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AssetTypeId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TickFormulaId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TickFormulaId",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "MarketId",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TickFormula",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");

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
    }
}
