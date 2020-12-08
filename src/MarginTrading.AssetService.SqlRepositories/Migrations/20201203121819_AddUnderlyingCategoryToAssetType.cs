using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddUnderlyingCategoryToAssetType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnderlyingCategoryId",
                schema: "dbo",
                table: "AssetTypes",
                nullable: false,
                defaultValue: "Stock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnderlyingCategoryId",
                schema: "dbo",
                table: "AssetTypes");
        }
    }
}
