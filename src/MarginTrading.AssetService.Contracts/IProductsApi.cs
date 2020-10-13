using System.Threading.Tasks;
using JetBrains.Annotations;
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
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> UpdateAsync([NotNull] string productId,
            [Body] UpdateProductRequest request);

        /// <summary>
        /// Deletes existing Product
        /// </summary>
        [Delete("/api/products/{productId}")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> DeleteAsync([NotNull] string productId,
            [Body] DeleteProductRequest request);

        /// <summary>
        /// Gets Product by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Get("/api/products/{productId}")]
        Task<GetProductByIdResponse> GetByIdAsync([NotNull] string productId);

        /// <summary>
        /// Gets all Products with pagination
        /// </summary>
        [Get("/api/products")]
        Task<GetProductsResponse> GetAllAsync([Query(CollectionFormat.Multi)] GetProductsRequest request, [Query] int skip = default,
            [Query] int take = 20);
        
        /// <summary>
        /// Gets products count for particular request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Get("/api/products/counter")]
        Task<GetProductsCountResponse> GetAllCountAsync([Query(CollectionFormat.Multi)] GetProductsRequest request);

        /// <summary>
        /// Updates product frozen status
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <returns></returns>
        [Put("/api/products/{productId}/frozen-status")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> ChangeFrozenStatus(string productId, ChangeProductFrozenStatusRequest request);
    }
}