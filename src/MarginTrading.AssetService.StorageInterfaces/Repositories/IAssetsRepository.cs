// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IAssetsRepository
    {
        Task<IReadOnlyList<string>> GetDuplicatedIsinsAsync(string[] isins);
        Task<IReadOnlyList<DiscontinuedProduct>> GetDiscontinuedProductsAsync();
        Task<IReadOnlyList<IAsset>> GetAsync();
        Task<IAsset> GetAsync(string assetId);
        Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null);
    }
}
