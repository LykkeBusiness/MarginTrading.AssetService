// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.Chaos;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Startup;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.Services.Caches;
using MarginTrading.AssetService.Services.Mapping;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;
using MarginTrading.AssetService.Services.Validations.Products;
using MarginTrading.AssetService.Settings.Candles;
using MarginTrading.AssetService.Settings.ServiceSettings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Module = Autofac.Module;
using SqlRepos = MarginTrading.AssetService.SqlRepositories.Repositories;

namespace MarginTrading.AssetService.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AssetServiceSettings> _settings;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AssetServiceSettings> settings)
        {
            _settings = settings;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // still required for some middlewares
            builder.Register(ctx => new LykkeLoggerAdapter<ServiceModule>(ctx.Resolve<ILogger<ServiceModule>>()))
                .As<ILog>()
                .SingleInstance();
            
            builder.RegisterInstance(_settings.CurrentValue.TradingInstrumentDefaults).AsSelf().SingleInstance();
 
            builder.RegisterInstance(_settings.CurrentValue.LegalEntityDefaults).AsSelf().SingleInstance(); 
            
            builder.RegisterInstance(_settings.CurrentValue.RequestLoggerSettings).SingleInstance(); 
            
            builder.RegisterInstance(_settings.CurrentValue.Platform).SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.CandlesSharding ?? new CandlesShardingSettings())
                .SingleInstance();  
            
            builder.RegisterInstance(_settings.CurrentValue.DefaultRateSettings).SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.TradingConditionsDefaults).SingleInstance();

            builder.RegisterType<HealthService>().As<IHealthService>().SingleInstance();

            builder.RegisterType<StartupManager>().As<IStartupManager>();

            builder.RegisterType<ShutdownManager>().As<IShutdownManager>();

            builder.RegisterType<EventSender>().As<IEventSender>()
                .WithParameters(new[]
                {
                    new NamedParameter("settingsChangedConnectionString",
                        _settings.CurrentValue.SettingsChangedRabbitMqSettings.ConnectionString),
                    new NamedParameter("settingsChangedExchangeName",
                        _settings.CurrentValue.SettingsChangedRabbitMqSettings.ExchangeName),
                })
                .SingleInstance();
            
            builder.RegisterType<SystemClock>().As<ISystemClock>().SingleInstance();
            
            builder.RegisterType<ConvertService>().As<IConvertService>().SingleInstance();
            
            builder.RegisterType<RabbitMqService>().As<IRabbitMqService>().SingleInstance();
            
            builder.RegisterType<MaintenanceModeService>().As<IMaintenanceModeService>().SingleInstance();

            builder.RegisterType<MarketDayOffService>().As<IMarketDayOffService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .SingleInstance();

            builder.RegisterType<ClientProfilesService>()
                .As<IClientProfilesService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .SingleInstance();

            builder.RegisterType<AssetTypesService>()
                .As<IAssetTypesService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .SingleInstance();

            builder.RegisterType<ClientProfileSettingsService>().As<IClientProfileSettingsService>().SingleInstance();
            builder.RegisterType<AuditService>().As<IAuditService>().SingleInstance();
            builder.RegisterType<MarketSettingsService>().As<IMarketSettingsService>().SingleInstance();

            builder.RegisterType<ProductAddOrUpdateValidationAndEnrichment>().AsSelf().SingleInstance();
            builder.RegisterType<ProductsService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProductsDiscontinueService>().AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterType<GuidIdentityGenerator>().AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterType<ProductCategoriesService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<TickFormulaService>().As<ITickFormulaService>().SingleInstance();

            //TODO need to change with impl
            builder.RegisterType<FakeTradingService>().As<ITradingService>().SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.Cqrs.ContextNames).AsSelf().SingleInstance();
            
            builder.RegisterType<CqrsMessageSender>()
                .As<ICqrsMessageSender>()
                .SingleInstance();

            builder.RegisterType<CqrsEntityChangedSender>()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.RegisterChaosKitty(_settings.CurrentValue.ChaosKitty);

            builder.RegisterType<RateSettingsService>()
                .As<IRateSettingsService>()
                .SingleInstance();

            builder.RegisterType<CurrenciesService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ProductCategoryStringService>()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.RegisterAssemblyTypes(typeof(MarginTrading.AssetService.Services.AssemblyDummy).Assembly)
                .Where(t => t.Name.EndsWith("Validation"))
                .AsSelf();

            builder.RegisterType<AssetPairService>()
                .As<IAssetPairService>()
                .SingleInstance();

            builder.RegisterType<MarketsService>()
                .As<IMarketsService>()
                .SingleInstance();

            builder.RegisterType<TradingConditionsService>()
                .As<ITradingConditionsService>()
                .SingleInstance();

            builder.RegisterType<TradingInstrumentsService>()
                .As<ITradingInstrumentsService>()
                .SingleInstance();

            builder.RegisterType<ScheduleSettingsService>()
                .As<IScheduleSettingsService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .SingleInstance();

            builder.RegisterType<UnderlyingsCache>()
                .As<IUnderlyingsCache>()
                .SingleInstance()
                .IfNotRegistered(typeof(IUnderlyingsCache));

            builder.RegisterType<LegacyAssetsService>()
                .As<ILegacyAssetsService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .WithParameter(new NamedParameter("assetTypesWithZeroInterestRate", _settings.CurrentValue.AssetTypesWithZeroInterestRates))
                .SingleInstance();

            builder.RegisterType<LegacyAssetCache>()
                .As<ILegacyAssetsCache>()
                .SingleInstance();

            builder.RegisterType<LegacyAssetsCacheUpdater>()
                .As<ILegacyAssetsCacheUpdater>()
                .WithParameter(new NamedParameter("assetTypesWithZeroInterestRate", _settings.CurrentValue.AssetTypesWithZeroInterestRates))
                .SingleInstance();

            builder.RegisterType<UnderlyingChangedHandler>()
                .AsSelf()
                .SingleInstance();      
            
            builder.RegisterType<BrokerSettingsChangedHandler>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<SettlementCurrencyService>()
                .As<ISettlementCurrencyService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.BrokerId))
                .SingleInstance();

            builder.RegisterType<UnderlyingCategoriesCache>()
                .As<IUnderlyingCategoriesCache>()
                .SingleInstance();

            builder.RegisterType<MarketScheduleResolver>()
                .AsSelf()
                .SingleInstance();

            RegisterRepositories(builder);

            builder.Populate(_services);
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            if (_settings.CurrentValue.Db.StorageMode == StorageMode.SqlServer)
            {
                if (string.IsNullOrEmpty(_settings.CurrentValue.Db.DataConnString))
                {
                    throw new Exception($"{nameof(_settings.CurrentValue.Db.DataConnString)} must have a value if StorageMode is SqlServer");
                }

                var connectionStringParameter = new NamedParameter("connectionString", 
                    _settings.CurrentValue.Db.DataConnString);
                
                builder.RegisterType<SqlRepos.AssetsRepository>()
                    .As<IAssetsRepository>()
                    .WithParameter(connectionStringParameter)
                    .SingleInstance();

                builder.RegisterType<SqlRepos.TradingRoutesRepository>()
                    .As<ITradingRoutesRepository>()
                    .WithParameter(connectionStringParameter)
                    .SingleInstance();
                
                builder.RegisterType<SqlRepos.OperationExecutionInfoRepository>()
                    .As<IOperationExecutionInfoRepository>()
                    .WithParameter(connectionStringParameter)
                    .SingleInstance();
                
                builder.RegisterType<SqlRepos.BlobRepository>()
                    .As<IMarginTradingBlobRepository>()
                    .WithParameter(connectionStringParameter)
                    .SingleInstance();

                builder.RegisterType<SqlRepos.AuditRepository>()
                    .As<IAuditRepository>()
                    .SingleInstance();

                builder.RegisterType<SqlRepos.ClientProfilesRepository>()
                    .As<IClientProfilesRepository>()
                    .SingleInstance();

                builder.RegisterSingletonIfNotRegistered<SqlRepos.AssetTypesRepository, IAssetTypesRepository>();

                builder.RegisterType<SqlRepos.ClientProfileSettingsRepository>()
                    .As<IClientProfileSettingsRepository>()
                    .SingleInstance();

                builder.RegisterType<SqlRepos.ProductsRepository>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                builder.RegisterType<SqlRepos.MarketSettingsRepository>()
                    .As<IMarketSettingsRepository>()
                    .SingleInstance();

                builder.RegisterType<SqlRepos.CurrenciesRepository>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                builder.RegisterType<SqlRepos.TickFormulaRepository>()
                    .As<ITickFormulaRepository>()
                    .SingleInstance();
                
                builder.RegisterType<SqlRepos.ProductCategoriesRepository>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
            else if (_settings.CurrentValue.Db.StorageMode == StorageMode.Azure)
            {
                throw new InvalidOperationException("Azure storage mode is not supported");
            }
        }
    }
}
