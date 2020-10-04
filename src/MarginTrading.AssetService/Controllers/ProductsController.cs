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
        public async Task<GetProductsResponse> GetAllAsync(int skip = default, int take = 20)
        {
            // if take == 0 return all rows
            var result = take == default
                ? await _productsService.GetAllAsync()
                : await _productsService.GetByPageAsync(skip, take);

            var response = new GetProductsResponse
            {
                Products = result.Value
                    .Select(p => _convertService.Convert<Product, ProductContract>(p))
                    .ToList()
            };

            return response;
        }

        [HttpPut("{productId}/frozen-status")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductsErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductsErrorCodesContract>> ChangeFrozenStatus(string productId, ChangeProductFrozenStatusRequest request)
        {
            var freezeInfo = _convertService.Convert<ProductFreezeInfoContract, ProductFreezeInfo>(request.FreezeInfo);
            
            var correlationId = this.TryGetCorrelationId();

            if (!request.IsFrozen && request.FreezeInfo != null)
            {
                return new ErrorCodeResponse<ProductsErrorCodesContract>()
                {
                    ErrorCode = ProductsErrorCodesContract.CanOnlySetFreezeInfoForFrozenProduct,
                }; 
            }
            
            var result = await _productsService.ChangeFrozenStatus(productId, request.IsFrozen, request.ForceFreezeIfAlreadyFrozen, freezeInfo, request.UserName, correlationId);

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