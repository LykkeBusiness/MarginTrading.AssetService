using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.ProductCategories;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface IProductCategoriesApi
    {
        /// <summary>
        /// Tries to create a list of product categories by transforming the category string and creating all subcategories
        /// Already created categories are ignored
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/product-categories")]
        Task<ErrorCodeResponse<ProductCategoriesErrorCodesContract>> UpsertAsync(
            [Body] AddProductCategoryRequest request);

        /// <summary>
        /// Deletes a product category. Only works on leaf nodes that do not have attached products
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Delete("/api/product-categories/{id}")]
        Task<ErrorCodeResponse<ProductCategoriesErrorCodesContract>> DeleteAsync(string id,
            [Body] DeleteProductCategoryRequest request);

        /// <summary>
        /// Gets a list of all product categories 
        /// </summary>
        /// <returns></returns>
        [Get("/api/product-categories")]
        Task<GetProductCategoriesResponse> GetAllAsync();

        /// <summary>
        /// Gets a product category by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/product-categories/{id}")]
        Task<GetProductCategoryByIdResponse> GetByIdAsync(string id);
    }
}