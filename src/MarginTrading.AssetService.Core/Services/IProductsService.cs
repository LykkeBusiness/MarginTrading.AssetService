using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductsService
    {
        Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username);
        Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username);
        Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, string username);
        Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId);
        Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted);
        Task<Result<ProductsCounter, ProductsErrorCodes>> GetAllCountAsync(string[] mdsCodes, string[] productIds);
        Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted, bool? isDiscontinued, int skip = default, int take = 20);
        Task<Result<Product, ProductsErrorCodes>> ChangeFrozenStatus(string productId, bool isFrozen,
            bool forceFreezeIfAlreadyFrozen,
            ProductFreezeInfo freezeInfo, string userName);

        Task<Result<ProductsErrorCodes>> UpdateBatchAsync(List<Product> products, string username);
        Task<Result<ProductsErrorCodes>> DeleteBatchAsync(List<string> productIds, string username);
        Task<Result<ProductsErrorCodes>> DiscontinueBatchAsync(string[] productIds, string username);

        Task<Result<ProductsErrorCodes>> ChangeUnderlyingMdsCodeAsync(string oldMdsCode, string newMdsCode, string username);

        Task<Result<ProductsErrorCodes>> ChangeTradingDisabledAsync(string productId, bool tradingDisabled, string username);
    }
}