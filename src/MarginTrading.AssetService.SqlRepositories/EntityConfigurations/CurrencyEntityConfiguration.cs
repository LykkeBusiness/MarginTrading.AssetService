using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class CurrencyEntityConfiguration : IEntityTypeConfiguration<CurrencyEntity>
    {
        private const int MaxLength = 100;
        
        public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().HasMaxLength(MaxLength);
            builder.Property(x => x.InterestRateMdsCode).IsRequired().HasMaxLength(MaxLength);

            builder.Property(x => x.Accuracy).IsRequired();

            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}