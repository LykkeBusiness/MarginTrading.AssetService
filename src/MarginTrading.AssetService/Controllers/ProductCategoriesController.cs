using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/product-categories")]
    public class ProductCategoriesController : ControllerBase, IProductCategoriesApi
    {
        private readonly IProductCategoriesService _productCategoriesService;
        private readonly IConvertService _convertService;

        public ProductCategoriesController(IProductCategoriesService productCategoriesService,
            IConvertService convertService)
        {
            _productCategoriesService = productCategoriesService;
            _convertService = convertService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductCategoriesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductCategoriesErrorCodesContract>> UpsertAsync(
            [FromBody] AddProductCategoryRequest request)
        {
            var result =
                await _productCategoriesService.GetOrCreate(request.Category, request.UserName);

            var response = new ErrorCodeResponse<ProductCategoriesErrorCodesContract>();
            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductCategoriesErrorCodes, ProductCategoriesErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ProductCategoriesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ProductCategoriesErrorCodesContract>> DeleteAsync(string id,
            [FromBody] DeleteProductCategoryRequest request)
        {
            var result =
                await _productCategoriesService.DeleteAsync(id, request.UserName);

            var response = new ErrorCodeResponse<ProductCategoriesErrorCodesContract>();
            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<ProductCategoriesErrorCodes, ProductCategoriesErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetProductCategoriesResponse), (int) HttpStatusCode.OK)]
        public async Task<GetProductCategoriesResponse> GetAllAsync()
        {
            var result = await _productCategoriesService.GetAllAsync();

            var response = new GetProductCategoriesResponse
            {
                ProductCategories = result.Value
                    .Select(p => _convertService.Convert<ProductCategory, ProductCategoryContract>(p))
                    .ToList()
            };

            return response;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetProductCategoryByIdResponse), (int) HttpStatusCode.OK)]
        public async Task<GetProductCategoryByIdResponse> GetByIdAsync(string id)
        {
            var result = await _productCategoriesService.GetByIdAsync(id);

            var response = new GetProductCategoryByIdResponse();

            if (result.IsSuccess)
            {
                response.ProductCategory =
                    _convertService.Convert<ProductCategory, ProductCategoryContract>(result.Value);
            }
            else
            {
                response.ErrorCode =
                    _convertService.Convert<ProductCategoriesErrorCodes, ProductCategoriesErrorCodesContract>(
                        result.Error.GetValueOrDefault());
            }

            return response;
        }

        [HttpPost("validate")]
        [ProducesResponseType(typeof(ValidateProductCategoriesResponse), (int) HttpStatusCode.OK)]
        public async Task<ValidateProductCategoriesResponse> Validate(ValidateProductCategoriesRequest request)
        {
            var pairs = request.Pairs
                .Select(p => _convertService.Convert<ProductAndCategoryPairContract, ProductAndCategoryPair>(p))
                .ToList();

            var errorMessages = await _productCategoriesService.Validate(pairs);
            
            var response = new ValidateProductCategoriesResponse
            {
                ErrorMessages = errorMessages
            };

            return response;
        }
    }
}