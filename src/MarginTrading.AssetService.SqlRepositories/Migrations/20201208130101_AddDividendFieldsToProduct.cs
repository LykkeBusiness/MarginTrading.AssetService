using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddDividendFieldsToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Dividends871M",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DividendsLong",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DividendsShort",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dividends871M",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DividendsLong",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DividendsShort",
                schema: "dbo",
                table: "Products");
        }
    }
}
