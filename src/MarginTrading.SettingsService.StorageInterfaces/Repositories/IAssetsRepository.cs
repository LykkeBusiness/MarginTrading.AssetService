using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetsRepository : IGenericCrudRepository<IAssetEntity>
    {
    }
}
