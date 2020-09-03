using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class RegenerateClientProfileAndAssetTypesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RegulatoryTypeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientProfiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RegulatoryProfileId = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientProfileSettings",
                schema: "dbo",
                columns: table => new
                {
                    ClientProfileId = table.Column<string>(nullable: false),
                    AssetTypeId = table.Column<string>(nullable: false),
                    Margin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionFeesFloor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionFeesCap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionFeesRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancingFeesRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnBehalfFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProfileSettings", x => new { x.ClientProfileId, x.AssetTypeId });
                    table.ForeignKey(
                        name: "FK_ClientProfileSettings_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalSchema: "dbo",
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientProfileSettings_ClientProfiles_ClientProfileId",
                        column: x => x.ClientProfileId,
                        principalSchema: "dbo",
                        principalTable: "ClientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProfileSettings_AssetTypeId",
                schema: "dbo",
                table: "ClientProfileSettings",
                column: "AssetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientProfileSettings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AssetTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ClientProfiles",
                schema: "dbo");
        }
    }
}
