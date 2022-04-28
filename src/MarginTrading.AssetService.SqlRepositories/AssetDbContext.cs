using System.Data.Common;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.SqlRepositories
{
    public class AssetDbContext : MsSqlContext
    {
        private const string Schema = "dbo";

        internal DbSet<ClientProfileSettingsEntity> ClientProfileSettings { get; set; }
        internal DbSet<AssetTypeEntity> AssetTypes { get; set; }
        internal DbSet<ClientProfileEntity> ClientProfiles { get; set; }
        internal DbSet<AuditEntity> AuditTrail { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        internal DbSet<MarketSettingsEntity> MarketSettings { get; set; }        
        internal DbSet<ProductCategoryEntity> ProductCategories { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        internal DbSet<TickFormulaEntity> TickFormulas { get; set; }
        
#if DEBUG
        private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
            builder => builder.AddConsole());  
#endif

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

        protected override void OnLykkeConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnLykkeConfiguring(optionsBuilder);
#if DEBUG
            optionsBuilder
                .UseLoggerFactory(LoggerFactory)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
#endif
        }
    }
}