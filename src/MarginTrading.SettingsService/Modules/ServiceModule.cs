using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Repositories;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Services;
using MarginTrading.SettingsService.Settings.ServiceSettings;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace MarginTrading.SettingsService.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<MarginTradingSettingsServiceSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<MarginTradingSettingsServiceSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).As<ILog>().SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.TradingInstrumentDefaults).AsSelf().SingleInstance();
 
            builder.RegisterInstance(_settings.CurrentValue.LegalEntityDefaults).AsSelf().SingleInstance(); 

            builder.RegisterInstance(_settings.CurrentValue.LegalEntityDefaults).AsSelf().SingleInstance();

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
            
            //TODO need to change with impl
            builder.RegisterType<FakeTradingService>().As<ITradingService>().SingleInstance();

            RegisterRepositories(builder);

            builder.Populate(_services);
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            var connstrParameter = new NamedParameter("connectionStringManager", 
                _settings.Nested(x => x.Db.MarginTradingConnString));
            
            builder.RegisterType<AssetPairsRepository>()
                .As<IAssetPairsRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<AssetsRepository>()
                .As<IAssetsRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<MarketRepository>()
                .As<IMarketRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<ScheduleSettingsRepository>()
                .As<IScheduleSettingsRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<TradingConditionsRepository>()
                .As<ITradingConditionsRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<TradingInstrumentsRepository>()
                .As<ITradingInstrumentsRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
            
            builder.RegisterType<TradingRoutesRepository>()
                .As<ITradingRoutesRepository>()
                .WithParameter(connstrParameter)
                .SingleInstance();
        }
    }
}
