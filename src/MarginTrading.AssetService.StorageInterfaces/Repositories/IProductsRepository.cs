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
        Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync();
        Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(int skip = default, int take = 20);
        Task<bool> UnderlyingHasOnlyOneProduct(string mdsCode, string productId);
        Task<Result<Product, ProductsErrorCodes>> ChangeFrozenStatus(string productId,
            bool isFrozen,
            byte[] valueTimestamp,
            ProductFreezeInfo freezeInfo);
        Task<Dictionary<string, string>> GetProductAssetTypeMapAsync(IEnumerable<string> productIds = null);
        Task<PaginatedResponse<Product>> GetPagedWithFilterAsync(string nameOrIdFilter, int skip = default,
            int take = 20);

        Task<PaginatedResponse<Product>> GetPagedAsync(int skip = default, int take = 20);
        Task<IReadOnlyList<Product>> GetWithFilterAsync(string nameOrIdFilter);
        Task<IReadOnlyList<Product>> GetByProductsIdsAsync(IEnumerable<string> productIds = null);
        Task<Result<Product, ProductsErrorCodes>> ChangeSuspendFlagAsync(string id, bool value);
    }
}