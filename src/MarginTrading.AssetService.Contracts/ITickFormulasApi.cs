using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.TickFormula;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface ITickFormulasApi
    {
        /// <summary>
        /// Get tick formula by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/tick-formula/{id}")]
        Task<GetTickFormulaByIdResponse> GetByIdAsync(string id);

        /// <summary>
        /// Get all tick formulas
        /// </summary>
        /// <returns></returns>
        [Get("/api/tick-formula")]
        Task<GetAllTickFormulasResponse> GetAllAsync();

        /// <summary>
        /// Adds new tick formula
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/tick-formula")]
        Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> AddAsync([Body] AddTickFormulaRequest request);

        /// <summary>
        /// Updates existing tick formula
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Put("/api/tick-formula/{id}")]
        Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> UpdateAsync([Body] UpdateTickFormulaRequest request, string id);

        /// <summary>
        /// Delete tick formula
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Delete("/api/tick-formula/{id}")]
        Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> DeleteAsync(string id, [Query]string username);
    }
}