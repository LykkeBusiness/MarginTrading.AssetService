using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class ClientProfileEntityConfiguration : IEntityTypeConfiguration<ClientProfileEntity>
    {
        public void Configure(EntityTypeBuilder<ClientProfileEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RegulatoryProfileId).IsRequired();
        }
    }
}