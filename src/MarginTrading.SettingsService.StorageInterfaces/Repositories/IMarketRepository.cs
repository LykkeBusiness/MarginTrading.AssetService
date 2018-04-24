using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IMarketRepository : IGenericCrudRepository<Market>
    {
    }
}
