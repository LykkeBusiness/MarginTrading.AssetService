using System.Data.Common;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories
{
    public class AssetDbContext : MsSqlContext
    {
        private const string Schema = "dbo";

        internal DbSet<BrokerRegulatorySettingsEntity> BrokerRegulatorySettings { get; set; }
        internal DbSet<BrokerRegulatoryTypeEntity> BrokerRegulatoryTypes { get; set; }
        internal DbSet<BrokerRegulatoryProfileEntity> BrokerRegulatoryProfiles { get; set; }
        internal DbSet<AuditEntity> AuditTrail { get; set; }

        // Used for EF migrations
        [UsedImplicitly]
        public AssetDbContext() : base(Schema)
        {
        }
        public AssetDbContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public AssetDbContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }

        public AssetDbContext(DbConnection dbConnection) : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetDbContext).Assembly);
        }
    }
}