using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
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
        private readonly IConvertService _convertService;

        public AssetTypesController(IAssetTypesService assetTypesService, IConvertService convertService)
        {
            _assetTypesService = assetTypesService;
            _convertService = convertService;
        }

        /// <summary>
        /// Check if there is any asset type with this regulatory type id
        /// </summary>
        /// <returns></returns>
        [HttpGet("any/assigned-to-regulatory-type/{regulatoryTypeId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync([FromRoute][Required] string regulatoryTypeId)
        {
            var response = await _assetTypesService.IsRegulatoryTypeAssignedToAnyAssetTypeAsync(regulatoryTypeId);

            return response;
        }

        /// <summary>
        /// Get asset type by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetAssetTypeByIdResponse), (int)HttpStatusCode.OK)]
        public async Task<GetAssetTypeByIdResponse> GetAssetTypeByIdAsync([FromRoute][Required] string id)
        {
            var response = new GetAssetTypeByIdResponse();

            var type = await _assetTypesService.GetByIdAsync(id);

            if (type == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
                return response;
            }

            response.AssetType = _convertService.Convert<AssetType, AssetTypeContract>(type);

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
                AssetTypes = types.Select(t => _convertService.Convert<AssetType,AssetTypeContract>(t)).ToList()
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

            try
            {
                var model = _convertService.Convert<AddAssetTypeRequest, AssetTypeWithTemplate>(request);
                await _assetTypesService.InsertAsync(model, request.Username);
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
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (RegulationConstraintViolationException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulationConstraintViolation;
            }
            catch (UnderlyingCategoryDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.UnderlyingCategoryDoesNotExist;
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
            ([FromRoute][Required] string id, [FromBody] UpdateAssetTypeRequest request)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            var model = _convertService.Convert<UpdateAssetTypeRequest, AssetType>(request);
            model.Id = id;

            try
            {
                await _assetTypesService.UpdateAsync(model, request.Username);
            }
            catch (AlreadyExistsException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AlreadyExist;
            }
            catch (AssetTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
            }
            catch (BrokerSettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.BrokerSettingsDoNotExist;
            }
            catch (RegulatoryTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatoryTypeInMdmIsMissing;
            }
            catch (RegulatorySettingsDoNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulatorySettingsAreMissing;
            }
            catch (RegulationConstraintViolationException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.RegulationConstraintViolation;
            }
            catch (UnderlyingCategoryDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.UnderlyingCategoryDoesNotExist;
            }

            return response;
        }

        /// <summary>
        /// Delete asset type
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<ClientProfilesErrorCodesContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> DeleteAssetTypeAsync([FromRoute][Required] string id, [FromQuery][Required] string username)
        {
            var response = new ErrorCodeResponse<ClientProfilesErrorCodesContract>();

            try
            {
                await _assetTypesService.DeleteAsync(id, username);
            }
            catch (AssetTypeDoesNotExistException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.AssetTypeDoesNotExist;
            }
            catch (CannotDeleteAssetTypeAssignedToAnyProductException)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.CannotDeleteAssetTypeAssignedToAnyProduct;
            }

            return response;
        }
    }
}