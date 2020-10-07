using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IProductCategoriesRepository
    {
        Task<Result<ProductCategoriesErrorCodes>> InsertAsync(ProductCategory category);
        Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, byte[] timestamp);
        Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id);
        Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync();
        Task<bool> CategoryHasProducts(string category);
        Task<IReadOnlyList<ProductCategory>> GetByIdsAsync(IEnumerable<string> ids);
    }
}