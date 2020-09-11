using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<string>(maxLength: 400, nullable: false),
                    AssetType = table.Column<string>(maxLength: 400, nullable: true),
                    Category = table.Column<string>(maxLength: 400, nullable: true),
                    Comments = table.Column<string>(maxLength: 400, nullable: true),
                    ContractSize = table.Column<int>(maxLength: 400, nullable: false),
                    IsinLong = table.Column<string>(maxLength: 400, nullable: true),
                    IsinShort = table.Column<string>(maxLength: 400, nullable: true),
                    Issuer = table.Column<string>(maxLength: 400, nullable: true),
                    Market = table.Column<string>(maxLength: 400, nullable: true),
                    MarketMakerAssetAccountId = table.Column<string>(maxLength: 400, nullable: true),
                    MaxOrderSize = table.Column<int>(nullable: false),
                    MinOrderSize = table.Column<int>(nullable: false),
                    MaxPositionSize = table.Column<int>(nullable: false),
                    MinOrderDistancePercent = table.Column<decimal>(nullable: false),
                    MinOrderEntryInterval = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: true),
                    NewsId = table.Column<string>(maxLength: 400, nullable: true),
                    Keywords = table.Column<string>(maxLength: 400, nullable: true),
                    PublicationRic = table.Column<string>(maxLength: 400, nullable: true),
                    SettlementCurrency = table.Column<string>(maxLength: 400, nullable: true),
                    ShortPosition = table.Column<bool>(nullable: false),
                    Tags = table.Column<string>(maxLength: 400, nullable: true),
                    TickFormula = table.Column<string>(maxLength: 400, nullable: true),
                    UnderlyingMdsCode = table.Column<string>(maxLength: 400, nullable: true),
                    ForceId = table.Column<string>(maxLength: 400, nullable: true),
                    Parity = table.Column<int>(nullable: false),
                    OvernightMarginMultiplier = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products",
                schema: "dbo");
        }
    }
}
