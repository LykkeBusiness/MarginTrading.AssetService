using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    public partial class ActualDiscontinuedDate_As_ExpiryDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Product  SET Product.[ActualDiscontinuedDate] = Underlying.ExpiryDate  " +
                                 "FROM [dbo].[Products] AS Product  " +
                                 "INNER JOIN [mdm].[Underlyings] as Underlying  " +
                                 "ON Product.UnderlyingMdsCode = Underlying.MdsCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Products]\n  SET [ActualDiscontinuedDate] = NULL");
        }
    }
}
