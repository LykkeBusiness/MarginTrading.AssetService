using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.ClientProfiles;
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
    /// Manages client profiles
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/client-profiles")]
    public class ClientProfilesController : ControllerBase, IClientProfilesApi
    {
        private readonly IClientProfilesService _regulatoryProfilesService;
        private readonly IMapper _mapper;

        public ClientProfilesController(IClientProfilesService regulatoryProfilesService, IMapper mapper)
        {
            _regulatoryProfilesService = regulatoryProfilesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get client profile by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetClientProfileByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetClientProfileByIdResponse> GetClientProfileByIdAsync([FromRoute] Guid id)
        {
            var response = new GetClientProfileByIdResponse();

            var profile = await _regulatoryProfilesService.GetByIdAsync(id);

            if (profile == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
                return response;
            }

            response.ClientProfile = _mapper.Map<ClientProfileContract>(profile);

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
                ClientProfiles = _mapper.Map<IReadOnlyList<ClientProfileContract>>(profiles)
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

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryProfilesService.InsertAsync(_mapper.Map<ClientProfileWithTemplate>(request), request.Username, correlationId);
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
            ([FromRoute][Required] Guid id, [FromBody] UpdateClientProfileRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<ClientProfile>(request);
            model.Id = id;

            try
            {
                await _regulatoryProfilesService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (ClientProfileDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
            }

            return response;
        }

        /// <summary>
        /// Delete a client profile
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> DeleteClientProfileAsync([FromRoute] Guid id, [FromQuery] string username)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryProfilesService.DeleteAsync(id, username, correlationId);
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