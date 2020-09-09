using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        private const int MaxLength = 400;
        private const string DbDecimal = "decimal(18,2)";
        
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.ProductId);

            builder.HasIndex(x => x.Name);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ProductId).HasMaxLength(MaxLength);
            builder.Property(x => x.AssetType).HasMaxLength(MaxLength);
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
            builder.Property(x => x.UnderlyingMdsCode).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.ForceId).HasMaxLength(MaxLength);

            builder.Property(x => x.MinOrderDistancePercent).HasColumnType(DbDecimal);
            builder.Property(x => x.MinOrderEntryInterval).HasColumnType(DbDecimal);
            builder.Property(x => x.OvernightMarginMultiplier).HasColumnType(DbDecimal);
            
            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}