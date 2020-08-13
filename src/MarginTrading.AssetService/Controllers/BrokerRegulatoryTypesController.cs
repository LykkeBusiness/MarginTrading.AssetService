using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes;
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
    /// Manages broker regulatory types
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/broker-regulatory-types")]
    public class BrokerRegulatoryTypesController : ControllerBase
    {
        private readonly IBrokerRegulatoryTypesService _regulatoryTypesService;
        private readonly IMapper _mapper;

        public BrokerRegulatoryTypesController(IBrokerRegulatoryTypesService regulatoryTypesService, IMapper mapper)
        {
            _regulatoryTypesService = regulatoryTypesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get broker regulatory type by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetBrokerRegulatoryTypeByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetBrokerRegulatoryTypeByIdResponse> GetRegulatoryTypeByIdAsync([FromRoute] Guid id)
        {
            var response = new GetBrokerRegulatoryTypeByIdResponse();

            var type = await _regulatoryTypesService.GetByIdAsync(id);

            if (type == null)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryTypeDoesNotExist;
                return response;
            }

            response.BrokerRegulatoryType = _mapper.Map<BrokerRegulatoryTypeContract>(type);

            return response;
        }

        /// <summary>
        /// Get all broker regulatory types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllBrokerRegulatoryTypesResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllBrokerRegulatoryTypesResponse> GetRegulatoryTypesByRegulationAsync()
        {
            var types = await _regulatoryTypesService.GetAllAsync();

            return new GetAllBrokerRegulatoryTypesResponse
            {
                BrokerRegulatoryTypes = _mapper.Map<IReadOnlyList<BrokerRegulatoryTypeContract>>(types)
            };
        }

        /// <summary>
        /// Adds new broker regulatory type to the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> AddRegulatoryTypeAsync([FromBody] AddBrokerRegulatoryTypeRequest request)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryTypesService.InsertAsync(_mapper.Map<BrokerRegulatoryTypeWithTemplate>(request), request.Username, correlationId);
            }
            catch (RegulatoryTypeDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryTypeDoesNotExist;
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.AlreadyExist;
            }

            return response;
        }

        /// <summary>
        /// Updates existing broker regulatory type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> UpdateRegulatoryTypeAsync
            ([FromRoute][Required] Guid id, [FromBody] UpdateBrokerRegulatoryTypeRequest request)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<BrokerRegulatoryType>(request);
            model.Id = id;

            try
            {
                await _regulatoryTypesService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.AlreadyExist;
            }
            catch (RegulatoryTypeDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryTypeDoesNotExist;
            }

            return response;
        }

        /// <summary>
        /// Delete broker regulatory type
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<BrokerRegulationsErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<BrokerRegulationsErrorCodesContract>> DeleteRegulatoryTypeAsync([FromRoute] Guid id, [FromQuery] string username)
        {
            var response = new ErrorCodeResponse<BrokerRegulationsErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _regulatoryTypesService.DeleteAsync(id, username, correlationId);
            }
            catch (RegulatoryTypeDoesNotExistException)
            {
                response.ErrorCode = BrokerRegulationsErrorCodesContract.BrokerRegulatoryTypeDoesNotExist;
            }

            return response;
        }
    }
}