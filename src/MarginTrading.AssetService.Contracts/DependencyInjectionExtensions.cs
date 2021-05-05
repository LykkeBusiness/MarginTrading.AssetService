using MarginTrading.AssetService.Contracts.ClientProfiles;
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
    }
}