using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class ProductCategoryEntityConfiguration : IEntityTypeConfiguration<ProductCategoryEntity>
    {
        private const int MaxLength = 400;
        
        public void Configure(EntityTypeBuilder<ProductCategoryEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(MaxLength);
            builder.Property(x => x.LocalizationToken).IsRequired().HasMaxLength(MaxLength);

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}