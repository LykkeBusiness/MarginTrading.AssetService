using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IGenericCrudRepository<TD>
    {
        Task<IReadOnlyList<TD>> GetAsync();
        Task<IReadOnlyList<TD>> GetAsync(Func<TD, bool> filter);
        Task<TD> GetAsync(string id);
        Task InsertAsync(TD obj);
        Task ReplaceAsync(TD obj);
        Task<bool> DeleteAsync(string id);
    }
}
