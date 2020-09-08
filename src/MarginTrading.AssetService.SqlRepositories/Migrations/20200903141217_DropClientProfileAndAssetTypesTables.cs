using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class DropClientProfileAndAssetTypesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegulatoryTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegulatoryProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    ClientProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExecutionFeesCap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionFeesFloor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionFeesRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancingFeesRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Margin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnBehalfFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IX_AssetTypes_NormalizedName",
                schema: "dbo",
                table: "AssetTypes",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientProfiles_NormalizedName",
                schema: "dbo",
                table: "ClientProfiles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientProfileSettings_AssetTypeId",
                schema: "dbo",
                table: "ClientProfileSettings",
                column: "AssetTypeId");
        }
    }
}
