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
    }
}