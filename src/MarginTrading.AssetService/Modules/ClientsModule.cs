using Autofac;
using Lykke.HttpClientGenerator;
using Lykke.SettingsReader;
using Lykke.Snow.Mdm.Contracts.Api;
using MarginTrading.AssetService.Settings;
using MarginTrading.AssetService.Settings.ServiceSettings;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Modules
{
    public class ClientsModule : Module
    {
        private readonly AppSettings _appSettings;

        public ClientsModule(IReloadingManager<AppSettings> reloadingManager)
        {
            _appSettings = reloadingManager.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterClientWithName<IRegulatoryTypesApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IRegulationsApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IRegulatoryProfilesApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IRegulatorySettingsApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IBrokerSettingsApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IUnderlyingsApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
            RegisterClientWithName<IUnderlyingCategoriesApi>(builder, "Mdm", _appSettings.MarginTradingAssetService.MdmService);
        }

        private static void RegisterClientWithName<TApi>(ContainerBuilder builder, string name,
            ServiceSettings serviceSettings)
            where TApi : class
        {
            builder.RegisterClient<TApi>(serviceSettings.ServiceUrl,
                config =>
                {
                    var httpClientGeneratorBuilder = config.WithServiceName<ProblemDetails>($"{name} [{serviceSettings.ServiceUrl}]");

                    if (!string.IsNullOrEmpty(serviceSettings.ApiKey))
                    {
                        httpClientGeneratorBuilder = httpClientGeneratorBuilder.WithApiKey(serviceSettings.ApiKey);
                    }

                    return httpClientGeneratorBuilder;
                });
        }

    }
}
