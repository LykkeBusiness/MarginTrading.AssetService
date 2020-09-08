using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ITickFormulaService
    {
        Task<ITickFormula> GetByIdAsync(string id);
        Task<IReadOnlyList<ITickFormula>> GetAllAsync();
        Task<Result<TickFormulaErrorCodes>> AddAsync(ITickFormula model, string username, string correlationId);
        Task<Result<TickFormulaErrorCodes>> UpdateAsync(ITickFormula model, string username, string correlationId);
        Task<Result<TickFormulaErrorCodes>> DeleteAsync(string id, string username, string correlationId);
    }
}