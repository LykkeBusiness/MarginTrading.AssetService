using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddMarketSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketSettings",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    MICCode = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: true),
                    DividendsLong = table.Column<decimal>(type: "decimal(18,13)", nullable: false),
                    DividendsShort = table.Column<decimal>(type: "decimal(18,13)", nullable: false),
                    Dividends871M = table.Column<decimal>(type: "decimal(18,13)", nullable: false),
                    Open = table.Column<TimeSpan>(nullable: false),
                    Close = table.Column<TimeSpan>(nullable: false),
                    Timezone = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                schema: "dbo",
                columns: table => new
                {
                    MarketSettingsId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => new { x.Date, x.MarketSettingsId });
                    table.ForeignKey(
                        name: "FK_Holidays_MarketSettings_MarketSettingsId",
                        column: x => x.MarketSettingsId,
                        principalSchema: "dbo",
                        principalTable: "MarketSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_MarketSettingsId",
                schema: "dbo",
                table: "Holidays",
                column: "MarketSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketSettings_NormalizedName",
                schema: "dbo",
                table: "MarketSettings",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holidays",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MarketSettings",
                schema: "dbo");
        }
    }
}
