using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface ITickFormulaRepository
    {
        Task<ITickFormula> GetByIdAsync(string id);
        Task<IReadOnlyList<ITickFormula>> GetAllAsync();
        Task<Result<TickFormulaErrorCodes>> AddAsync(ITickFormula model);
        Task<Result<TickFormulaErrorCodes>> UpdateAsync(ITickFormula model);
        Task<Result<TickFormulaErrorCodes>> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<bool> AssignedToAnyProductAsync(string id);
        Task<IReadOnlyList<ITickFormula>> GetByIdsAsync(IEnumerable<string> ids);
    }
}