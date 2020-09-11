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

        internal DbSet<ClientProfileSettingsEntity> ClientProfileSettings { get; set; }
        internal DbSet<AssetTypeEntity> AssetTypes { get; set; }
        internal DbSet<ClientProfileEntity> ClientProfiles { get; set; }
        internal DbSet<AuditEntity> AuditTrail { get; set; }
        internal DbSet<ProductEntity> Products { get; set; }
        internal DbSet<MarketSettingsEntity> MarketSettings { get; set; }        
        internal DbSet<ProductCategoryEntity> ProductCategories { get; set; }
        internal DbSet<CurrencyEntity> Currencies { get; set; }
        internal DbSet<TickFormulaEntity> TickFormulas { get; set; }

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