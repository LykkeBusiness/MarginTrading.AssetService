// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.AssetService.StorageInterfaces
{
    public interface IGenericCrudRepository<TD>
    {
        Task<IReadOnlyList<TD>> GetAsync();
        Task<IReadOnlyList<TD>> GetAsync(Func<TD, bool> filter);
        Task<TD> GetAsync(string rowKey, string partitionKey = null);
        Task<bool> TryInsertAsync(TD obj);
        Task ReplaceAsync(TD obj);
        Task<bool> DeleteAsync(string rowKey, string partitionKey = null);
    }
}
