using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class AuditEntityConfiguration : IEntityTypeConfiguration<AuditEntity>
    {
        public void Configure(EntityTypeBuilder<AuditEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Timestamp).IsRequired();
            builder.Property(x => x.DataType).IsRequired();
            builder.Property(x => x.DataReference).IsRequired();
            builder.Property(x => x.DataDiff).IsRequired();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.CorrelationId).IsRequired();
        }
    }
}