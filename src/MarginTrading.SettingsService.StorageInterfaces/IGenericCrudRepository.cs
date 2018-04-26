using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.StorageInterfaces
{
    public interface IGenericCrudRepository<TD>
    {
        Task<IReadOnlyList<TD>> GetAsync();
        Task<IReadOnlyList<TD>> GetAsync(Func<TD, bool> filter);
        Task<TD> GetAsync(string rowKey, string partitionKey = null);
        Task InsertAsync(TD obj);
        Task ReplaceAsync(TD obj);
        Task<bool> DeleteAsync(string rowKey, string partitionKey = null);
    }
}
