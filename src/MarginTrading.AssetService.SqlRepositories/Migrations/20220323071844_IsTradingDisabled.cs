using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class IsTradingDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "TradingDisabled",
                newName: "IsTradingDisabled",
                schema: "dbo",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "IsTradingDisabled",
                newName: "TradingDisabled",
                schema: "dbo",
                table: "Products");
        }
    }
}
