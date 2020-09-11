using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class AddProductCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 400, nullable: false),
                    LocalizationToken = table.Column<string>(maxLength: 400, nullable: false),
                    ParentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentId",
                schema: "dbo",
                table: "ProductCategories",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "dbo");
        }
    }
}
