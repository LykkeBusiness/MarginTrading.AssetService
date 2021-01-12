using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class HalfWorkingDayFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketSchedule_Schedule",
                schema: "dbo",
                table: "MarketSettings",
                nullable: true);
            
            ExecuteDataMigration(migrationBuilder);
            
            migrationBuilder.DropColumn(
                name: "Close",
                schema: "dbo",
                table: "MarketSettings");

            migrationBuilder.DropColumn(
                name: "Open",
                schema: "dbo",
                table: "MarketSettings");

            migrationBuilder.DropColumn(
                name: "Timezone",
                schema: "dbo",
                table: "MarketSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketSchedule_Schedule",
                schema: "dbo",
                table: "MarketSettings");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Close",
                schema: "dbo",
                table: "MarketSettings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Open",
                schema: "dbo",
                table: "MarketSettings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                schema: "dbo",
                table: "MarketSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
        
        private static void ExecuteDataMigration(MigrationBuilder migrationBuilder)
        {
            var scriptFilePath = Path.Combine(
                AppContext.BaseDirectory, 
                "Migrations", 
                "Scripts",
                "20210112191618_ConvertSchedule.sql");

            var sqlScript = File.ReadAllText(scriptFilePath);

            migrationBuilder.Sql(sqlScript);
        }
    }
}
