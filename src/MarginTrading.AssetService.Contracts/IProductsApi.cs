using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.Products;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface IProductsApi
    {
        /// <summary>
        /// Creates new Product
        /// </summary>
        [Post("/api/products")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> AddAsync([Body] AddProductRequest request);

        /// <summary>
        /// Updates existing Product
        /// </summary>
        [Put("/api/products/{productId}")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> UpdateAsync([Required] string productId,
            [Body] UpdateProductRequest request);

        /// <summary>
        /// Deletes existing Product
        /// </summary>
        [Delete("/api/products/{productId}")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> DeleteAsync([Required] string productId,
            [Query] string username);

        /// <summary>
        /// Gets Product by mds code
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Get("/api/products/{productId}")]
        Task<GetProductByIdResponse> GetByIdAsync([Required] string productId);

        /// <summary>
        /// Gets all Products with pagination
        /// </summary>
        [Get("/api/products")]
        Task<GetProductsResponse> GetAllAsync([Query] int skip = default, [Query] int take = 20);
    }
}