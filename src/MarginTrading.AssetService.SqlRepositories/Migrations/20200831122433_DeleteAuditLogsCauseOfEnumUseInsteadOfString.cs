using Microsoft.EntityFrameworkCore.Migrations;

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class DeleteAuditLogsCauseOfEnumUseInsteadOfString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[AuditTrail]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
