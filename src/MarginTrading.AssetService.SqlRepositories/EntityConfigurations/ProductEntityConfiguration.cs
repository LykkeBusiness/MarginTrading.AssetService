using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasOne(x => x.TradingCurrency)
                .WithMany()
                .HasForeignKey(x => x.TradingCurrencyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.Market)
                .WithMany()
                .IsRequired()
                .HasForeignKey(x => x.MarketId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(x => x.AssetType)
                .WithMany()
                .HasForeignKey(x => x.AssetTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(x => x.TickFormula)
                .WithMany()
                .HasForeignKey(x => x.TickFormulaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ProductId).HasMaxLength(MaxLength);
            builder.Property(x => x.Comments).HasMaxLength(MaxLength);
            builder.Property(x => x.ContractSize).HasMaxLength(MaxLength);
            builder.Property(x => x.IsinLong).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.IsinShort).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.Issuer).HasMaxLength(MaxLength);
            builder.Property(x => x.MarketMakerAssetAccountId).HasMaxLength(MaxLength);
            builder.Property(x => x.Name).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.NewsId).HasMaxLength(MaxLength);
            builder.Property(x => x.Keywords).HasMaxLength(MaxLength);
            builder.Property(x => x.PublicationRic).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.SettlementCurrency).HasMaxLength(MaxLength);
            builder.Property(x => x.Tags).HasMaxLength(MaxLength);
            builder.Property(x => x.UnderlyingMdsCode).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.ForceId).HasMaxLength(MaxLength).IsRequired();
            builder.Property(x => x.TradingCurrencyId).HasMaxLength(100).IsRequired();
            // json data
            builder.Property(x => x.FreezeInfo).HasMaxLength(2000);

            builder.Property(x => x.MaxOrderSize).IsRequired();
            builder.Property(x => x.MinOrderSize).IsRequired();
            builder.Property(x => x.MaxPositionSize).IsRequired();
            builder.Property(x => x.Parity).IsRequired();

            builder.Property(x => x.MinOrderDistancePercent).HasColumnType(DbDecimal).IsRequired();
            builder.Property(x => x.MinOrderEntryInterval).HasColumnType(DbDecimal).IsRequired();
            builder.Property(x => x.OvernightMarginMultiplier).HasColumnType(DbDecimal).IsRequired();

            builder.Property(x => x.ShortPosition).IsRequired();
            builder.Property(x => x.IsSuspended).IsRequired();
            builder.Property(x => x.IsFrozen).IsRequired();
            builder.Property(x => x.IsDiscontinued).IsRequired();

            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.IsStarted).IsRequired();

            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}