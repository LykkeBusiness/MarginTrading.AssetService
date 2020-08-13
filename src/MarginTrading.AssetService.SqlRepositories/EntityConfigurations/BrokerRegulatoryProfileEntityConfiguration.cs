using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class BrokerRegulatoryProfileEntityConfiguration : IEntityTypeConfiguration<BrokerRegulatoryProfileEntity>
    {
        public void Configure(EntityTypeBuilder<BrokerRegulatoryProfileEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.NormalizedName ).IsUnique();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.NormalizedName).IsRequired();
            builder.Property(x => x.RegulatoryProfileId).IsRequired();
        }
    }
}