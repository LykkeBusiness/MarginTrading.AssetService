using System.Threading.Tasks;
using JetBrains.Annotations;

using Lykke.Contracts.Responses;

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
        [Post("/api/products/list")]
        Task<GetProductsResponse> GetAllAsync([Body] GetProductsRequest request);
        
        /// <summary>
        /// Update product freeze status
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <param name="request">Freeze details</param>
        /// <returns></returns>
        [Put("/api/products/{productId}/frozen-status")]
        Task<ChangeProductFrozenStatusResponse> ChangeFrozenStatusAsync(string productId, ChangeProductFrozenStatusRequest request);

        /// <summary>
        /// Update multiple products with same freeze details
        /// </summary>
        /// <returns></returns>
        [Put("/api/products/frozen-status")]
        Task<ChangeMultipleProductFrozenStatusResponse> ChangeFrozenStatusMultipleAsync(ChangeMultipleProductFrozenStatusRequest request);

        /// <summary>
        /// Gets products count for particular request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Get("/api/products/counter")]
        Task<GetProductsCountResponse> GetAllCountAsync([Query] GetProductsCountRequest request);

        /// <summary>
        /// Updates a list of products
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/products/batch")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> UpdateBatchAsync(
            [Body] UpdateProductBatchRequest request);

        /// <summary>
        /// Deletes a list of products by their productIds
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Delete("/api/products/batch")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> DeleteBatchAsync(
            [Body] DeleteProductBatchRequest request);

        [Put("/api/products/batch/discontinue")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> MarkMultipleAsDiscontinuedAsync(
            [Body] MarkProductsAsDiscontinuedRequest request);

        [Put("/api/products/{productId}/disable-trading")]
        Task<ErrorCodeResponse<ProductsErrorCodesContract>> ChangeTradingDisabledStatusAsync(
            string productId,
            [Body] ChangeProductTradingDisabledStatusRequest request);
    }
}