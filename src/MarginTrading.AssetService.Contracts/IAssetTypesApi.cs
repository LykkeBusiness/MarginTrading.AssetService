using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface IAssetTypesApi
    {
        /// <summary>
        /// Get asset type by id
        /// </summary>
        /// <returns></returns>
        [Get("/api/asset-types/{id}")]
        Task<GetAssetTypeByIdResponse> GetAssetTypeByIdAsync(string id);

        /// <summary>
        /// Get all asset types
        /// </summary>
        /// <returns></returns>
        [Get("/api/asset-types")]
        Task<GetAllAssetTypesResponse> GetAssetTypesAsync();

        /// <summary>
        /// Adds new asset type to the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/asset-types")]
        Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> AddAssetTypeAsync([Body] AddAssetTypeRequest request);

        /// <summary>
        /// Updates existing asset type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/asset-types/{id}")]
        Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateAssetTypeAsync(string id,
            [Body] UpdateAssetTypeRequest request);

        /// <summary>
        /// Delete asset type
        /// </summary>
        /// <returns></returns>
        [Delete("/api/asset-types/{id}")]
        Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> DeleteAssetTypeAsync(string id, [Query] string username);

        /// <summary>
        /// Check if there is any asset type with this regulatory type id
        /// </summary>
        /// <returns></returns>
        [Get("/api/asset-types/any/assigned-to-regulatory-type/{regulatoryTypeId}")]
        Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync(string regulatoryTypeId);
    }
}