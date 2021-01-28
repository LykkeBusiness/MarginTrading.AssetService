using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class StartDateRemoveDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: null,
                oldDefaultValue: DateTime.UtcNow);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: DateTime.UtcNow,
                oldDefaultValue: null);
        }
    }
}
