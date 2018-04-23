using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IGenericCrudRepository<TEntity>
    {
        Task<IReadOnlyList<TEntity>> GetAsync();
        Task<TEntity> GetAsync(string id);
        Task InsertAsync(TEntity obj);
        Task ReplaceAsync(TEntity obj);
        Task<TEntity> DeleteAsync(string id);
    }
}
