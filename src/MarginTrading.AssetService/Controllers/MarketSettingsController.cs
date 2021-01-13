// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Manages market settings
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/market-settings")]
    public class MarketSettingsController : ControllerBase, IMarketSettingsApi
    {
        private readonly IMarketSettingsService _marketSettingsService;
        private readonly IConvertService _convertService;

        public MarketSettingsController(IMarketSettingsService marketSettingsService, IConvertService convertService)
        {
            _marketSettingsService = marketSettingsService;
            _convertService = convertService;
        }

        /// <summary>
        /// Get market settings by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetMarketSettingsByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetMarketSettingsByIdResponse> GetByIdAsync([Required]string id)
        {
            var result = await _marketSettingsService.GetByIdAsync(id);

            var response = new GetMarketSettingsByIdResponse();

            if (result == null)
            {
                response.ErrorCode = MarketSettingsErrorCodesContract.MarketSettingsDoNotExist;
                return response;
            }

            response.MarketSettings = _convertService.Convert<MarketSettings, MarketSettingsContract>(result);

            return response;
        }

        /// <summary>
        /// Get all market settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllMarketSettingsResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllMarketSettingsResponse> GetAllMarketSettingsAsync()
        {
            var result = await _marketSettingsService.GetAllMarketSettingsAsync();

            return new GetAllMarketSettingsResponse
            {
                MarketSettingsContracts = result.Select(x => _convertService.Convert<MarketSettings, MarketSettingsContract>(x)).ToList()
            };
        }

        /// <summary>
        /// Adds new market settings
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<MarketSettingsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> AddMarketSettingsAsync([FromBody] AddMarketSettingsRequest request)
        {
            var model = _convertService.Convert<AddMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>(request);

            var correlationId = this.TryGetCorrelationId();
            var result = await _marketSettingsService.AddAsync(model, request.Username, correlationId);

            var response = new ErrorCodeResponse<MarketSettingsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode = _convertService.Convert<MarketSettingsErrorCodes, MarketSettingsErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        /// <summary>
        /// Updates existing market settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<MarketSettingsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> UpdateMarketSettingsAsync([FromBody] UpdateMarketSettingsRequest request, [Required] string id)
        {
            var model = _convertService.Convert<UpdateMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>(request);
            model.Id = id;

            var correlationId = this.TryGetCorrelationId();

            var result = await _marketSettingsService.UpdateAsync(model, request.Username, correlationId);

            var response = new ErrorCodeResponse<MarketSettingsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode = _convertService.Convert<MarketSettingsErrorCodes, MarketSettingsErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        /// <summary>
        /// Deletes market settings
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<MarketSettingsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> DeleteMarketSettingsAsync([Required]string id, [Required] string username)
        {
            var correlationId = this.TryGetCorrelationId();
            var result = await _marketSettingsService.DeleteAsync(id, username, correlationId);

            var response = new ErrorCodeResponse<MarketSettingsErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode = _convertService.Convert<MarketSettingsErrorCodes, MarketSettingsErrorCodesContract>(result.Error.Value);
            }

            return response;
        }
    }
}