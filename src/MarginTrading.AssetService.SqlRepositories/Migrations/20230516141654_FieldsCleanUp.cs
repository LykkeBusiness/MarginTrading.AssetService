using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class FieldsCleanUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinOrderEntryInterval",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "dbo",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderEntryInterval",
                schema: "dbo",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "dbo",
                table: "Products",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
