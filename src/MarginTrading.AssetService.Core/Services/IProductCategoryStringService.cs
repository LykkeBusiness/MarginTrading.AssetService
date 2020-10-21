using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductCategoryStringService
    {
        Task<Result<ProductCategoryName, ProductCategoriesErrorCodes>> Create(string category);
    }
}