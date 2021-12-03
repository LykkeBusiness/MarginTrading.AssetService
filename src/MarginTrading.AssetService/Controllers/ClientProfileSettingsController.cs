using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
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
        private readonly IClientProfilesService _clientProfilesService;
        private readonly IClientProfileSettingsService _clientProfileSettingsService;
        private readonly IConvertService _convertService;

        public ClientProfileSettingsController(IClientProfileSettingsService clientProfileSettingsService,
            IConvertService convertService,
            IClientProfilesService clientProfilesService)
        {
            _clientProfileSettingsService = clientProfileSettingsService;
            _convertService = convertService;
            _clientProfilesService = clientProfilesService;
        }

        /// <summary>
        /// Check if changes in regulatory settings will violate constraints for any entity
        /// </summary>
        /// <returns></returns>
        [HttpGet("will-violate-regulation-constraint")]
        [ProducesResponseType(typeof(bool), (int) HttpStatusCode.OK)]
        public async Task<bool> WillViolateRegulationConstraintAsync(
            [FromQuery] CheckRegulationConstraintViolationRequest request)
        {
            var model =
                _convertService.Convert<CheckRegulationConstraintViolationRequest, RegulatorySettingsDto>(request);

            var response = await _clientProfileSettingsService
                .WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(model);

            return response;
        }

        /// <summary>
        /// Get client profile settings by ids
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile/{profileId}/type/{typeId}")]
        [ProducesResponseType(typeof(GetClientProfileSettingsByIdsResponse), (int) HttpStatusCode.OK)]
        public async Task<GetClientProfileSettingsByIdsResponse> GetClientProfileSettingsByIdsAsync(
            [FromRoute] [Required] string profileId,
            [FromRoute] [Required] string typeId)
        {
            var response = new GetClientProfileSettingsByIdsResponse();

            var settings = await _clientProfileSettingsService.GetByIdAsync(profileId, typeId);

            if (settings == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileSettingsDoNotExist;
                return response;
            }

            response.ClientProfileSettings =
                _convertService.Convert<ClientProfileSettings, ClientProfileSettingsContract>(settings);

            return response;
        }

        /// <summary>
        /// Get all client profile settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllClientProfileSettingsResponse), (int) HttpStatusCode.OK)]
        public async Task<GetAllClientProfileSettingsResponse> GetClientProfileSettingsByRegulationAsync()
        {
            var clientProfileSettings = await _clientProfileSettingsService.GetAllAsync();

            return new GetAllClientProfileSettingsResponse
            {
                ClientProfileSettings = clientProfileSettings.Select(s =>
                    _convertService.Convert<ClientProfileSettings, ClientProfileSettingsContract>(s)).ToList()
            };
        }

        /// <summary>
        /// Get default profile settings for asset type id
        /// </summary>
        /// <param name="typeId">The asset type identifier</param>
        /// <returns></returns>
        [HttpGet("default/{typeId}")]
        [ProducesResponseType(typeof(GetDefaultClientProfileSettingsResponse), (int) HttpStatusCode.OK)]
        public async Task<GetDefaultClientProfileSettingsResponse> GetDefaultProfileSettings(string typeId)
        {
            var clientProfile = await _clientProfilesService.GetDefaultAsync();
            if (clientProfile == null)
                throw new InvalidOperationException("Default client profile is not found");

            var clientProfileSettings = await _clientProfileSettingsService.GetByIdAsync(clientProfile.Id, typeId);

            return new GetDefaultClientProfileSettingsResponse
            {
                ClientProfileSettings =
                    _convertService.Convert<ClientProfileSettings, ClientProfileSettingsContract>(clientProfileSettings)
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
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateClientProfileSettingsAsync(
            [FromBody] UpdateClientProfileSettingsRequest request,
            [FromRoute] [Required] string profileId,
            [FromRoute] [Required] string typeId)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var model = _convertService.Convert<UpdateClientProfileSettingsRequest, ClientProfileSettings>(request);
            model.ClientProfileId = profileId;
            model.AssetTypeId = typeId;

            try
            {
                await _clientProfileSettingsService.UpdateAsync(model, request.Username);
            }
            catch (InvalidMarginValueException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidMarginValue;
            }
            catch (CannotSetToAvailableException)
            {
                response.ErrorCode =
                    ClientProfilesErrorCodesContract.CannotSetToAvailableBecauseOfRegulatoryRestriction;
            }
            catch (ClientSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileSettingsDoNotExist;
            }
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (InvalidOnBehalfFeeException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.InvalidOnBehalfFeeValue;
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