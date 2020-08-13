using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class BrokerRegulatorySettingsEntityConfiguration : IEntityTypeConfiguration<BrokerRegulatorySettingsEntity>
    {
        public void Configure(EntityTypeBuilder<BrokerRegulatorySettingsEntity> builder)
        {
            builder.HasKey(x => new { ProfileId = x.BrokerProfileId, TypeId = x.BrokerTypeId });

            builder
                .HasOne(x => x.BrokerProfile)
                .WithMany()
                .HasForeignKey(x => x.BrokerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.BrokerType)
                .WithMany()
                .HasForeignKey(x => x.BrokerTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ExecutionFeesCap).IsRequired();
            builder.Property(x => x.ExecutionFeesFloor).IsRequired();
            builder.Property(x => x.ExecutionFeesRate).IsRequired();
            builder.Property(x => x.FinancingFeesRate).IsRequired();
            builder.Property(x => x.IsAvailable).IsRequired();
            builder.Property(x => x.MarginMin).IsRequired();
            builder.Property(x => x.PhoneFees).IsRequired();
        }
    }
}