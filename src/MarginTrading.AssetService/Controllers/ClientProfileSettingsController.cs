using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
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
    /// Manages client profile settings
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/client-profile-settings")]
    public class ClientProfileSettingsController : ControllerBase, IClientProfileSettingsApi
    {
        private readonly IClientProfileSettingsService _clientProfileSettingsService;
        private readonly IMapper _mapper;

        public ClientProfileSettingsController(IClientProfileSettingsService clientProfileSettingsService, IMapper mapper)
        {
            _clientProfileSettingsService = clientProfileSettingsService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get client profile settings by ids
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile/{profileId}/type/{typeId}")]
        [ProducesResponseType(typeof(GetClientProfileSettingsByIdsResponse), (int)HttpStatusCode.OK)]
        public async Task<GetClientProfileSettingsByIdsResponse> GetClientProfileSettingsByIdsAsync([FromRoute] Guid profileId, [FromRoute] Guid typeId)
        {
            var response = new GetClientProfileSettingsByIdsResponse();

            var settings = await _clientProfileSettingsService.GetByIdAsync(profileId, typeId);

            if (settings == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileSettingsDoNotExist;
                return response;
            }

            response.ClientProfileSettings = _mapper.Map<ClientProfileSettingsContract>(settings);

            return response;
        }

        /// <summary>
        /// Get all client profile settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllClientProfileSettingsResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllClientProfileSettingsResponse> GetClientProfileSettingsByRegulationAsync()
        {
            var regulatorySettings = await _clientProfileSettingsService.GetAllAsync();

            return new GetAllClientProfileSettingsResponse
            {
                ClientProfileSettings = _mapper.Map<IReadOnlyList<ClientProfileSettingsContract>>(regulatorySettings)
            };
        }

        /// <summary>
        /// Updates existing client settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="profileId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPut("profile/{profileId}/type/{typeId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateClientProfileSettingsAsync(
            [FromBody] UpdateClientProfileSettingsRequest request, [FromRoute][Required] Guid profileId, [FromRoute][Required] Guid typeId)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<ClientProfileSettings>(request);
            model.ClientProfileId = profileId;
            model.AssetTypeId = typeId;

            try
            {
                await _clientProfileSettingsService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (InvalidMinMarginValueException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidMarginMinValue;
            }
            catch (CannotSetToAvailableException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.CannotSetToAvailableBecauseOfRegulatoryRestriction;
            }
            catch (ClientSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileSettingsDoNotExist;
            }
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (InvalidPhoneFeesException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidPhoneFeesValue;
            }
            catch (InvalidExecutionFeesRateException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidExecutionFeesRate;
            }
            catch (InvalidExecutionFeesCapException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidExecutionFeesCap;
            }
            catch (InvalidExecutionFeesFloorException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidExecutionFeesFloor;
            }

            return response;
        }
    }
}