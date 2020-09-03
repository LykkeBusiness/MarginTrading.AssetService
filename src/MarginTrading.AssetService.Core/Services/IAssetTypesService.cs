using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAssetTypesService
    {
        Task InsertAsync(AssetTypeWithTemplate model, string username, string correlationId);
        Task UpdateAsync(AssetType model, string username, string correlationId);
        Task DeleteAsync(string id, string username, string correlationId);
        Task<AssetType> GetByIdAsync(string id);
        Task<IReadOnlyList<AssetType>> GetAllAsync();
    }
}