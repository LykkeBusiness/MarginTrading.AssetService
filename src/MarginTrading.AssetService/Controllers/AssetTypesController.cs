using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.AssetTypes;
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
    /// Manages asset types
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/asset-types")]
    public class AssetTypesController : ControllerBase, IAssetTypesApi
    {
        private readonly IAssetTypesService _assetTypesService;
        private readonly IMapper _mapper;

        public AssetTypesController(IAssetTypesService assetTypesService, IMapper mapper)
        {
            _assetTypesService = assetTypesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get asset type by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetAssetTypeByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAssetTypeByIdResponse> GetAssetTypeByIdAsync([FromRoute] Guid id)
        {
            var response = new GetAssetTypeByIdResponse();

            var type = await _assetTypesService.GetByIdAsync(id);

            if (type == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
                return response;
            }

            response.AssetType = _mapper.Map<AssetTypeContract>(type);

            return response;
        }

        /// <summary>
        /// Get all asset types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllAssetTypesResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAllAssetTypesResponse> GetAssetTypesAsync()
        {
            var types = await _assetTypesService.GetAllAsync();

            return new GetAllAssetTypesResponse
            {
                AssetTypes = _mapper.Map<IReadOnlyList<AssetTypeContract>>(types)
            };
        }

        /// <summary>
        /// Adds new asset type to the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> AddAssetTypeAsync([FromBody] AddAssetTypeRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _assetTypesService.InsertAsync(_mapper.Map<AssetTypeWithTemplate>(request), request.Username,
                    correlationId);
            }
            catch (AssetTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (BrokerSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.BrokerSettingsDoNotExist;
            }
            catch (RegulatoryTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatoryTypeInMdmIsMissing;

            }

            return response;
        }

        /// <summary>
        /// Updates existing asset type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateAssetTypeAsync
            ([FromRoute][Required] Guid id, [FromBody] UpdateAssetTypeRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            var model = _mapper.Map<AssetType>(request);
            model.Id = id;

            try
            {
                await _assetTypesService.UpdateAsync(model, request.Username, correlationId);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (AssetTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
            }

            return response;
        }

        /// <summary>
        /// Delete asset type
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> DeleteAssetTypeAsync([FromRoute] Guid id, [FromQuery] string username)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var correlationId = this.TryGetCorrelationId();

            try
            {
                await _assetTypesService.DeleteAsync(id, username, correlationId);
            }
            catch (AssetTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
            }

            return response;
        }
    }
}