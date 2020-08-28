using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        private const int MaxLength = 400;
        
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.ProductId);

            builder.Property(x => x.ProductId).HasMaxLength(MaxLength);
            builder.Property(x => x.AssetType).HasMaxLength(MaxLength);
            builder.Property(x => x.Category).HasMaxLength(MaxLength);
            builder.Property(x => x.Comments).HasMaxLength(MaxLength);
            builder.Property(x => x.ContractSize).HasMaxLength(MaxLength);
            builder.Property(x => x.IsinLong).HasMaxLength(MaxLength);
            builder.Property(x => x.IsinShort).HasMaxLength(MaxLength);
            builder.Property(x => x.Issuer).HasMaxLength(MaxLength);
            builder.Property(x => x.Market).HasMaxLength(MaxLength);
            builder.Property(x => x.MarketMakerAssetAccountId).HasMaxLength(MaxLength);
            builder.Property(x => x.Name).HasMaxLength(MaxLength);
            builder.Property(x => x.NewsId).HasMaxLength(MaxLength);
            builder.Property(x => x.Keywords).HasMaxLength(MaxLength);
            builder.Property(x => x.PublicationRic).HasMaxLength(MaxLength);
            builder.Property(x => x.SettlementCurrency).HasMaxLength(MaxLength);
            builder.Property(x => x.Tags).HasMaxLength(MaxLength);
            builder.Property(x => x.TickFormula).HasMaxLength(MaxLength);
            builder.Property(x => x.UnderlyingMdsCode).HasMaxLength(MaxLength);
            builder.Property(x => x.ForceId).HasMaxLength(MaxLength);
        }
    }
}