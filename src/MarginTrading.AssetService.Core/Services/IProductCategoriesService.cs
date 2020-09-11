using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductCategoriesService
    {
        Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, string username,
            string correlationId);

        Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync();

        Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id);

        /// <summary>
        /// <para /> Creates category hierarchy from category string
        /// <para /> Only creates categories if they do not exist
        /// </summary>
        /// <param name="category">Raw category string, e.g. "Stocks/Germany/DAX 30"</param>
        /// <param name="username"></param>
        /// <param name="correlationId"></param>
        /// <returns>Category leaf</returns>
        Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetOrCreate(string category,
            string username,
            string correlationId);
    }
}