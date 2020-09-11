// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class MarketSettingsEntityConfiguration : IEntityTypeConfiguration<MarketSettingsEntity>
    {
        public void Configure(EntityTypeBuilder<MarketSettingsEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.NormalizedName).IsUnique();

            builder
                .OwnsMany(x => x.Holidays,
                    x =>
                    {
                        x.HasKey(k => new {k.Date, k.MarketSettingsId});
                        x.ToTable("Holidays");
                        x.WithOwner()
                        .HasForeignKey(r => r.MarketSettingsId);
                    });

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Timezone).IsRequired();
            builder.Property(x => x.Open).IsRequired();
            builder.Property(x => x.Close).IsRequired();
            builder.Property(x => x.Dividends871M).IsRequired();
            builder.Property(x => x.DividendsLong).IsRequired();
            builder.Property(x => x.DividendsShort).IsRequired();

            builder.Property(p => p.Dividends871M)
                .HasColumnType("decimal(18,13)");
            builder.Property(p => p.DividendsLong)
                .HasColumnType("decimal(18,13)");
            builder.Property(p => p.DividendsShort)
                .HasColumnType("decimal(18,13)");
        }
    }
}