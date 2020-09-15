using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class ProductFreezeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreezeInfo",
                schema: "dbo",
                table: "Products",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscontinued",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFrozen",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreezeInfo",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDiscontinued",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFrozen",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                schema: "dbo",
                table: "Products");
        }
    }
}
