using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductsService
    {
        Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username, string correlationId);
        Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username, string correlationId);
        Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, string username, string correlationId);
        Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId);
        Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync();
        Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(int skip = default, int take = 20);
    }
}