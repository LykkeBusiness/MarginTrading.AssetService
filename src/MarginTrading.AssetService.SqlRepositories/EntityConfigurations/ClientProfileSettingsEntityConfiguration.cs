using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class ClientProfileSettingsEntityConfiguration : IEntityTypeConfiguration<ClientProfileSettingsEntity>
    {
        private const string DecimalType = "decimal(18,2)";
        public void Configure(EntityTypeBuilder<ClientProfileSettingsEntity> builder)
        {
            builder.HasKey(x => new { x.ClientProfileId, x.AssetTypeId });

            builder
                .HasOne(x => x.ClientProfile)
                .WithMany()
                .HasForeignKey(x => x.ClientProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.AssetType)
                .WithMany()
                .HasForeignKey(x => x.AssetTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ExecutionFeesCap).IsRequired();
            builder.Property(x => x.ExecutionFeesFloor).IsRequired();
            builder.Property(x => x.ExecutionFeesRate).IsRequired();
            builder.Property(x => x.FinancingFeesRate).IsRequired();
            builder.Property(x => x.IsAvailable).IsRequired();
            builder.Property(x => x.Margin).IsRequired();
            builder.Property(x => x.OnBehalfFee).IsRequired();

            builder.Property(p => p.ExecutionFeesCap)
                .HasColumnType(DecimalType);
            builder.Property(p => p.ExecutionFeesFloor)
                .HasColumnType(DecimalType);
            builder.Property(p => p.ExecutionFeesRate)
                .HasColumnType(DecimalType);
            builder.Property(p => p.FinancingFeesRate)
                .HasColumnType(DecimalType);
            builder.Property(p => p.Margin)
                .HasColumnType(DecimalType);
            builder.Property(p => p.OnBehalfFee)
                .HasColumnType(DecimalType);
        }
    }
}