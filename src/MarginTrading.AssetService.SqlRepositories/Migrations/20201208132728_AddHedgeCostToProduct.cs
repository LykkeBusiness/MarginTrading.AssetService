using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddHedgeCostToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsShort",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsLong",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Dividends871M",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,13)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HedgeCost",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HedgeCost",
                schema: "dbo",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsShort",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DividendsLong",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Dividends871M",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldNullable: true);
        }
    }
}
