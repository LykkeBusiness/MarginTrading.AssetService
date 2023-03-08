using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddExcludeSpreadFromProductCostsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeSpreadFromProductCosts",
                schema: "dbo",
                table: "AssetTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeSpreadFromProductCosts",
                schema: "dbo",
                table: "AssetTypes");
        }
    }
}
