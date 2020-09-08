using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductCategoriesService
    {
        Task<Result<ProductCategoriesErrorCodes>> UpsertAsync(string category, string username,
            string correlationId);

        Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, string username,
            string correlationId);

        Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync();
        Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id);
    }
}