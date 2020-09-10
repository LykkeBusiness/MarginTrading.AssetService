using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class ProductsTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "dbo",
                table: "Products",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                schema: "dbo",
                table: "Products",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "Products");
        }
    }
}
