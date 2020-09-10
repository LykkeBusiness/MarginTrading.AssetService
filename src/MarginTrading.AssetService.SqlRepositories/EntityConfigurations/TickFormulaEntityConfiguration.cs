using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.SqlRepositories.EntityConfigurations
{
    public class TickFormulaEntityConfiguration : IEntityTypeConfiguration<TickFormulaEntity>
    {
        public void Configure(EntityTypeBuilder<TickFormulaEntity> builder)
        {
            builder.HasKey(x => x.Id);

            var valueComparer = new ValueComparer<List<decimal>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())));

            builder
                .Property(e => e.PdlLadders)
                .HasConversion(
                    v => v.ToJson(false),
                    v => JsonConvert.DeserializeObject<List<decimal>>(v))
                .Metadata.SetValueComparer(valueComparer);

            builder
                .Property(e => e.PdlTicks)
                .HasConversion(
                    v => v.ToJson(false),
                    v => JsonConvert.DeserializeObject<List<decimal>>(v))
                .Metadata.SetValueComparer(valueComparer);
        }
    }
}