using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IProductsRepository
    {
        Task<Result<ProductsErrorCodes>> InsertAsync(Product product);
        Task<Result<ProductsErrorCodes>> UpdateAsync(Product product);
        Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, byte[] timestamp);
        Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId);
        Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(
            string mdsCodeFilter,
            string[] mdsCodes, string[] productIds,
            bool? isStarted = null, bool? isDiscontinued = null);
        Task<Result<ProductsCounter, ProductsErrorCodes>> GetAllCountAsync(string[] mdsCodes, string[] productIds);
        Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(
            string mdsCodeFilter,
            string[] mdsCodes, string[] productIds,
            bool? isStarted = null, bool? isDiscontinued = null, int skip = default, int take = 20);
        Task<Result<Product, ProductsErrorCodes>> ChangeFrozenStatus(string productId,
            bool isFrozen,
            byte[] valueTimestamp,
            ProductFreezeInfo freezeInfo);

        Task<Result<ProductsErrorCodes>> UpdateBatchAsync(List<Product> products);
        Task<Result<ProductsErrorCodes>> DeleteBatchAsync(Dictionary<string,byte[]> productIdsWithTimestamps);
        Task<Dictionary<string, string>> GetProductAssetTypeMapAsync(IEnumerable<string> productIds = null);
        Task<IReadOnlyList<Product>> GetByProductsIdsAsync(IEnumerable<string> productIds = null, bool startedOnly = true);
        Task<PaginatedResponse<Product>> GetPagedByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds,
            int skip = default, int take = 20);
        Task<IReadOnlyList<Product>> GetByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds);
        Task MarkAsDiscontinuedAsync(ICollection<string> productIds);
        Task<Result<Product, ProductsErrorCodes>> ChangeSuspendFlagAsync(string id, bool value);
        Task<Result<List<Product>, ProductsErrorCodes>> GetByUnderlyingMdsCodeAsync(string mdsCode);
        Task<(bool result, string id)> IsinExists(string isin, bool? isDiscontinued = null);
    }
}