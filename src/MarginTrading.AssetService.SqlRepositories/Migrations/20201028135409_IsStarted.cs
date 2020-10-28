using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class IsStarted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("update dbo.Products set IsStarted = '1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStarted",
                schema: "dbo",
                table: "Products");
        }
    }
}
