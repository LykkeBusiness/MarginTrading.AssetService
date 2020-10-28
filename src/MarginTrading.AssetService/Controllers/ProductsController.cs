using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Manages products
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/products")]
    public class ProductsController : ControllerBase, IProductsApi
    {
        private readonly IProductsService _productsService;
        private readonly IConvertService _convertService;

        public ProductsController(IProductsService productsService, IConvertService convertService)
        {
            _productsService = productsService;
            _convertService = convertService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> AddAsync(AddProductRequest request)
        {
            var product = _convertService.Convert<AddProductRequest, Product>(request);
            var correlationId = this.TryGetCorrelationId();
            var result = await _productsService.InsertAsync(product, request.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> UpdateAsync(string productId,
            UpdateProductRequest request)
        {
            var product = _convertService.Convert<UpdateProductRequest, Product>(request);
            product.ProductId = productId;
            var correlationId = this.TryGetCorrelationId();

            var result = await _productsService.UpdateAsync(product, request.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> DeleteAsync(string productId, [FromBody] DeleteProductRequest request)
        {
            var correlationId = this.TryGetCorrelationId();
            var result = await _productsService.DeleteAsync(productId, request.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(GetProductByIdResponse), (int) HttpStatusCode.OK)]
        public async Task<GetProductByIdResponse> GetByIdAsync(string productId)
        {
            var result = await _productsService.GetByIdAsync(productId);

            var response = new GetProductByIdResponse();

            if (result.IsSuccess)
            {
                response.Product = _convertService.Convert<Product, ProductContract>(result.Value);
            }
            else
            {
                response.ErrorCode = _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                    result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetProductsResponse), (int) HttpStatusCode.OK)]
        public async Task<GetProductsResponse> GetAllAsync([FromQuery] GetProductsRequest request, int skip = default, int take = 20)
        {
            // if take == 0 return all rows
            var result = take == default
                ? await _productsService.GetAllAsync(request?.MdsCodes, request?.ProductIds, request?.IsStarted)
                : await _productsService.GetByPageAsync(request?.MdsCodes, request?.ProductIds, request?.IsStarted,skip, take);

            var response = new GetProductsResponse
            {
                Products = result.Value
                    .Select(p => _convertService.Convert<Product, ProductContract>(p))
                    .ToList()
            };

            return response;
        }

        [HttpGet("counter")]
        [ProducesResponseType(typeof(GetProductsCountResponse), (int) HttpStatusCode.OK)]
        public async Task<GetProductsCountResponse> GetAllCountAsync([FromQuery] GetProductsRequest request)
        {
            var result = await _productsService.GetAllCountAsync(request.MdsCodes, request.ProductIds);

            return new GetProductsCountResponse {Count = result.Value.Counter};
        }

        [HttpPut("{productId}/frozen-status")]
        [ProducesResponseType(typeof(ChangeProductFrozenStatusResponse), (int) HttpStatusCode.OK)]
        public async Task<ChangeProductFrozenStatusResponse> ChangeFrozenStatusAsync(string productId, ChangeProductFrozenStatusRequest request)
        {
            var freezeInfo = _convertService.Convert<ProductFreezeInfoContract, ProductFreezeInfo>(request.FreezeInfo);
            
            var correlationId = this.TryGetCorrelationId();

            if (!request.IsFrozen && request.FreezeInfo != null)
            {
                return new ChangeProductFrozenStatusResponse()
                {
                    ErrorCode = ProductsErrorCodesContract.CanOnlySetFreezeInfoForFrozenProduct,
                }; 
            }
            
            var result = await _productsService.ChangeFrozenStatus(productId, request.IsFrozen, request.IsForceFreeze(), freezeInfo, request.UserName, correlationId);

            var response = new ChangeProductFrozenStatusResponse();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }
            else
            {
                response.Product = _convertService.Convert<Product, ProductContract>(result.Value);
            }

            return response;
        }

        [HttpPut("frozen-status")]
        [ProducesResponseType(typeof(ChangeMultipleProductFrozenStatusResponse), (int) HttpStatusCode.OK)]
        public async Task<ChangeMultipleProductFrozenStatusResponse> ChangeFrozenStatusMultipleAsync(
            ChangeMultipleProductFrozenStatusRequest request)
        {
            var freezeInfo =
                _convertService.Convert<ProductFreezeInfoContract, ProductFreezeInfo>(request.FreezeParameters
                    .FreezeInfo);

            var correlationId = this.TryGetCorrelationId();

            if (!request.FreezeParameters.IsFrozen && request.FreezeParameters.FreezeInfo != null)
            {
                return new ChangeMultipleProductFrozenStatusResponse
                {
                    ErrorCode = ProductsErrorCodesContract.CanOnlySetFreezeInfoForFrozenProduct,
                    Results = new Dictionary<string, ChangeProductFrozenStatusResponse>()
                };
            }

            var response = new ChangeMultipleProductFrozenStatusResponse
                {Results = new Dictionary<string, ChangeProductFrozenStatusResponse>()};

            foreach (var requestProductId in request.ProductIds)
            {
                var result = await _productsService.ChangeFrozenStatus(requestProductId,
                    request.FreezeParameters.IsFrozen, 
                    request.FreezeParameters.IsForceFreeze(), 
                    freezeInfo,
                    request.FreezeParameters.UserName, 
                    correlationId);

                var singleProductResponse = new ChangeProductFrozenStatusResponse();

                if (result.IsFailed)
                {
                    singleProductResponse.ErrorCode =
                        _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                            result.Error.GetValueOrDefault());
                }
                else
                {
                    singleProductResponse.Product = _convertService.Convert<Product, ProductContract>(result.Value);
                }

                response.Results.Add(requestProductId, singleProductResponse);
            }

            return response;
        }
        
        [HttpPut("batch")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> UpdateBatchAsync(
            [FromBody] UpdateProductBatchRequest request)
        {
            var products = request.Requests
                .Select(kvp =>
                {
                    var product = _convertService.Convert<UpdateProductRequest, Product>(kvp.Value);
                    product.ProductId = kvp.Key;
                    return product;
                })
                .ToList();

            var correlationId = this.TryGetCorrelationId();

            var result = await _productsService.UpdateBatchAsync(products,
                request.Requests.FirstOrDefault().Value.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();
            
            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
          
        }
        
        [HttpDelete("batch")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> DeleteBatchAsync(
            [FromBody] DeleteProductBatchRequest request)
        {
            var correlationId = this.TryGetCorrelationId();

            var result = await _productsService.DeleteBatchAsync(request.ProductIds.ToList(),
                request.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();
            
            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
          
        }

        /// <summary>
        /// Discontinue a batch of products
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("batch/discontinue")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> MarkMultipleAsDiscontinuedAsync([FromBody] MarkProductsAsDiscontinuedRequest request)
        {
            var correlationId = this.TryGetCorrelationId();

            var result = await _productsService.DiscontinueBatchAsync(request.ProductIds, request.UserName, correlationId);

            var response = new ErrorCodeResponse<ProductsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductsErrorCodes, ProductsErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }
    }
}