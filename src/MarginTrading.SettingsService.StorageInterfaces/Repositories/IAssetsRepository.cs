using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetsRepository : IGenericCrudRepository<Asset>
    {
    }
}
