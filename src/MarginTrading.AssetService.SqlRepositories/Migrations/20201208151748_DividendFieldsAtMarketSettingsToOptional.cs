using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class DividendFieldsAtMarketSettingsToOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsShort",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsLong",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Dividends871M",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsShort",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsLong",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Dividends871M",
                schema: "dbo",
                table: "MarketSettings",
                type: "decimal(18,13)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);
        }
    }
}
