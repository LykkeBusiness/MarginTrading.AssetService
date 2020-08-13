using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class BrokerRegulatoryTypeEntityConfiguration : IEntityTypeConfiguration<BrokerRegulatoryTypeEntity>
    {
        public void Configure(EntityTypeBuilder<BrokerRegulatoryTypeEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.NormalizedName).IsUnique();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.NormalizedName).IsRequired();
            builder.Property(x => x.RegulatoryTypeId).IsRequired();
        }
    }
}