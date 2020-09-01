using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.MarketSettings;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Api for market settings
    /// </summary>
    public interface IMarketSettingsApi
    {
        /// <summary>
        /// Get market settings by id
        /// </summary>
        /// <returns></returns>
        [Get("/api/market-settings/{id}")]
        Task<GetMarketSettingsByIdResponse> GetByIdAsync(string id);

        /// <summary>
        /// Get all market settings ids
        /// </summary>
        /// <returns></returns>
        [Get("/api/market-settings")]
        Task<GetAllMarketSettingsResponse> GetAllMarketSettingsAsync();

        /// <summary>
        /// Add market settings
        /// </summary>
        /// <returns></returns>
        [Post("/api/market-settings")]
        Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> AddMarketSettingsAsync(
            [Body] AddMarketSettingsRequest request);

        /// <summary>
        /// Update market settings
        /// </summary>
        /// <returns></returns>
        [Put("/api/market-settings/{id}")]
        Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> UpdateMarketSettingsAsync(
            [Body] UpdateMarketSettingsRequest request, [Required] string id);

        /// <summary>
        /// Add market settings
        /// </summary>
        /// <returns></returns>
        [Delete("/api/market-settings/{id}")]
        Task<ErrorCodeResponse<MarketSettingsErrorCodesContract>> DeleteMarketSettingsAsync(
            [Required] string id, [Query] [Required] string username);
    }
}