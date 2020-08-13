using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles;
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
    /// Manages broker regulatory profiles
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/broker-regulatory-profiles")]
    public class BrokerRegulatoryProfilesController : ControllerBase
    {
        private readonly IBrokerRegulatoryProfilesService _regulatoryProfilesService;
        private readonly IMapper _mapper;

        public BrokerRegulatoryProfilesController(IBrokerRegulatoryProfilesService regulatoryProfilesService, IMapper mapper)
        {
            _regulatoryProfilesService = regulatoryProfilesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get broker regulatory profile by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetBrokerRegulatoryProfileByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetBrokerRegulatoryProfileByIdResponse> GetRegulatoryProfileByIdAsync([FromRoute] Guid id)
        {
            var response = new GetBrokerRegulatoryProfileByIdResponse();

            var profile = await _regulatoryProfilesService.GetByIdAsync(id);

            if (profile == null)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryProfileDoesNotExist;
                return response;
            }

            response.BrokerRegulatoryProfile = _mapper.Map<BrokerRegulatoryProfileContract>(profile);

            return response;
        }

        /// <summary>
        /// Get all broker regulatory profiles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllBrokerRegulatoryProfilesResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllBrokerRegulatoryProfilesResponse> GetRegulatoryProfilesAsync()
        {
            var profiles = await _regulatoryProfilesService.GetAllAsync();

            return new GetAllBrokerRegulatoryProfilesResponse
            {
                BrokerRegulatoryProfiles = _mapper.Map<IReadOnlyList<BrokerRegulatoryProfileContract>>(profiles)
            };
        }

        /// <summary>
        /// Adds new broker regulatory profile to the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> AddRegulatoryProfileAsync([FromBody] AddBrokerRegulatoryProfileRequest request)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryProfilesService.InsertAsync(_mapper.Map<BrokerRegulatoryProfileWithTemplate>(request), request.Username, correlationId);
            }
            catch (RegulatoryProfileDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryProfileDoesNotExist;
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.AlreadyExist;
            }

            return response;
        }

        /// <summary>
        /// Updates existing broker regulatory profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> UpdateRegulatoryProfileAsync
            ([FromRoute][Required] Guid id, [FromBody] UpdateBrokerRegulatoryProfileRequest request)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<BrokerRegulatoryProfile>(request);
            model.Id = id;

            try
            {
                await _regulatoryProfilesService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.AlreadyExist;
            }
            catch (RegulatoryProfileDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryProfileDoesNotExist;
            }

            return response;
        }

        /// <summary>
        /// Delete a broker regulatory profile
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> DeleteRegulatoryProfileAsync([FromRoute] Guid id, [FromQuery] string username)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryProfilesService.DeleteAsync(id, username, correlationId);
            }
            catch (RegulatoryProfileDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryProfileDoesNotExist;
            }
            catch (CannotDeleteException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.CannotDeleteDefault;
            }

            return response;
        }
    }
}