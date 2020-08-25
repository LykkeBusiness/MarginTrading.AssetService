using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegulatoryTypeId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrail",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    CorrelationId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    DataType = table.Column<string>(nullable: false),
                    DataReference = table.Column<string>(nullable: false),
                    DataDiff = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientProfiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegulatoryProfileId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: false),
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
                    ClientProfileId = table.Column<Guid>(nullable: false),
                    AssetTypeId = table.Column<Guid>(nullable: false),
                    Margin = table.Column<decimal>(nullable: false),
                    ExecutionFeesFloor = table.Column<decimal>(nullable: false),
                    ExecutionFeesCap = table.Column<decimal>(nullable: false),
                    ExecutionFeesRate = table.Column<decimal>(nullable: false),
                    FinancingFeesRate = table.Column<decimal>(nullable: false),
                    OnBehalfFee = table.Column<decimal>(nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail",
                schema: "dbo");

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
