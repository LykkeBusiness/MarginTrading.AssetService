using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using Microsoft.Extensions.DependencyInjection;

namespace MarginTrading.AssetService.Contracts
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IClientProfileSettingsCache"/> in the .NET Core DI Container.
        /// Requires <see cref="IClientProfileSettingsApi"/> to be registered.
        /// </summary>
        /// <param name="services"></param>
        public static void AddClientProfileSettingsCache(this IServiceCollection services)
        {
            services.AddSingleton<IClientProfileSettingsCache, ClientProfileSettingsCache>();
        }
        
        /// <summary>
        /// Registers <see cref="IAssetTypeCache"/> in the .NET Core DI Container.
        /// Requires <see cref="IAssetTypesApi"/> to be registered.
        /// </summary>
        /// <param name="services"></param>
        public static void AddAssetTypesCache(this IServiceCollection services)
        {
            services.AddSingleton<IAssetTypeCache, AssetTypeCache>();
        }
    }
}