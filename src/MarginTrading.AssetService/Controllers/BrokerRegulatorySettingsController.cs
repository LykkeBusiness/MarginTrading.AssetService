using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts.BrokerRegulatorySettings;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Manages broker regulatory settings
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/broker-regulatory-settings")]
    public class BrokerRegulatorySettingsController : ControllerBase
    {
        private readonly IBrokerRegulatorySettingsService _regulatorySettingsService;
        private readonly IMapper _mapper;

        public BrokerRegulatorySettingsController(IBrokerRegulatorySettingsService regulatorySettingsService, IMapper mapper)
        {
            _regulatorySettingsService = regulatorySettingsService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get broker regulatory settings by ids
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile/{profileId}/type/{typeId}")]
        [ProducesResponseType(typeof(GetBrokerRegulatorySettingsByIdsResponse), (int)HttpStatusCode.OK)]
        public async Task<GetBrokerRegulatorySettingsByIdsResponse> GetRegulatorySettingsByIdsAsync([FromRoute] Guid profileId, [FromRoute] Guid typeId)
        {
            var response = new GetBrokerRegulatorySettingsByIdsResponse();

            var regulatorySettings = await _regulatorySettingsService.GetByIdAsync(profileId, typeId);

            if (regulatorySettings == null)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatorySettingsDoNotExist;
                return response;
            }

            response.BrokerRegulatorySettings = _mapper.Map<BrokerRegulatorySettingsContract>(regulatorySettings);

            return response;
        }

        /// <summary>
        /// Get all broker regulatory settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("{regulationId}/all")]
        [ProducesResponseType(typeof(GetAllBrokerRegulatorySettingsResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllBrokerRegulatorySettingsResponse> GetRegulatorySettingsByRegulationAsync()
        {
            var regulatorySettings = await _regulatorySettingsService.GetAllAsync();

            return new GetAllBrokerRegulatorySettingsResponse
            {
                BrokerRegulatorySettings = _mapper.Map<IReadOnlyList<BrokerRegulatorySettingsContract>>(regulatorySettings)
            };
        }

        /// <summary>
        /// Updates existing broker regulatory settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="profileId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPut("profile/{profileId}/type/{typeId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> UpdateRegulatorySettingsAsync(
            [FromBody] UpdateBrokerRegulatorySettingsRequest request, [FromRoute][Required] Guid profileId, [FromRoute][Required] Guid typeId)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<BrokerRegulatorySettings>(request);
            model.BrokerProfileId = profileId;
            model.BrokerTypeId = typeId;

            try
            {
                await _regulatorySettingsService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (InvalidMinMarginValueException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.InvalidMarginMinValue;
            }
            catch (BrokerRegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatorySettingsDoNotExist;
            }

            return response;
        }
    }
}