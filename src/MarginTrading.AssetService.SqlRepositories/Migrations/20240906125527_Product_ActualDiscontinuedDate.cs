using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class Product_ActualDiscontinuedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ActualDiscontinuedDate",
                schema: "dbo",
                table: "Products",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDiscontinuedDate",
                schema: "dbo",
                table: "Products");
        }
    }
}
