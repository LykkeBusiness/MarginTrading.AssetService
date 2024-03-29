﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.ClientProfiles;
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
    /// Manages client profiles
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/client-profiles")]
    public class ClientProfilesController : ControllerBase, IClientProfilesApi
    {
        private readonly IClientProfilesService _regulatoryProfilesService;
        private readonly IConvertService _convertService;

        public ClientProfilesController(IClientProfilesService regulatoryProfilesService, 
            IConvertService convertService)
        {
            _regulatoryProfilesService = regulatoryProfilesService;
            _convertService = convertService;
        }

        /// <summary>
        /// Check if there is any client profile with this regulatory profile id
        /// </summary>
        /// <returns></returns>
        [HttpGet("any/assigned-to-regulatory-profile/{regulatoryProfileId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<bool> IsRegulatoryProfileAssignedToAnyClientProfileAsync([FromRoute][Required] string regulatoryProfileId)
        {
            var response = await _regulatoryProfilesService.IsRegulatoryProfileAssignedToAnyClientProfileAsync(regulatoryProfileId);

            return response;
        }

        /// <summary>
        /// Get client profile by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetClientProfileByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetClientProfileByIdResponse> GetClientProfileByIdAsync([FromRoute][Required] string id)
        {
            var response = new GetClientProfileByIdResponse();

            var profile = await _regulatoryProfilesService.GetByIdAsync(id);

            if (profile == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
                return response;
            }

            response.ClientProfile = _convertService.Convert<ClientProfile, ClientProfileContract>(profile);

            return response;
        }

        /// <summary>
        /// Get all client profiles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllClientProfilesResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllClientProfilesResponse> GetClientProfilesAsync()
        {
            var profiles = await _regulatoryProfilesService.GetAllAsync();

            return new GetAllClientProfilesResponse
            {
                ClientProfiles = profiles.Select(p => _convertService.Convert<ClientProfile, ClientProfileContract>(p)).ToList()
            };
        }

        /// <summary>
        /// Adds new broker regulatory profile to the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> AddClientProfileAsync([FromBody] AddClientProfileRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            try
            {
                var clientProfile = new ClientProfileWithTemplate(
                    request.Id, 
                    request.RegulatoryProfileId,
                    request.ClientProfileTemplateId, 
                    request.IsDefault);
                await _regulatoryProfilesService.InsertAsync(clientProfile, request.Username);
            }
            catch (ClientProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (BrokerSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.BrokerSettingsDoNotExist;
            }
            catch (RegulatoryProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatoryProfileInMdmIsMissing;
            }
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (RegulationConstraintViolationException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulationConstraintViolation;
            }
            catch (ClientProfileNonDefaultUpdateForbiddenException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.NonDefaultUpdateForbidden;
            }

            return response;
        }

        /// <summary>
        /// Updates existing client profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateClientProfileAsync
            ([FromRoute][Required] string id, [FromBody] UpdateClientProfileRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            try
            {
                var clientProfile = new ClientProfile(id, request.RegulatoryProfileId, request.IsDefault);
                await _regulatoryProfilesService.UpdateAsync(clientProfile, request.Username);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (ClientProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
            }
            catch (BrokerSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.BrokerSettingsDoNotExist;
            }
            catch (RegulatoryProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatoryProfileInMdmIsMissing;
            }
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (RegulationConstraintViolationException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulationConstraintViolation;
            }
            catch (ClientProfileNonDefaultUpdateForbiddenException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.NonDefaultUpdateForbidden;
            }

            return response;
        }

        /// <summary>
        /// Delete a client profile
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> DeleteClientProfileAsync([FromRoute][Required] string id, [FromQuery][Required] string username)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            try
            {
                await _regulatoryProfilesService.DeleteAsync(id, username);
            }
            catch (ClientProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
            }
            catch (CannotDeleteException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.CannotDeleteDefault;
            }

            return response;
        }
    }
}