﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ITickFormulaService
    {
        Task<ITickFormula> GetByIdAsync(string id);
        Task<IReadOnlyList<ITickFormula>> GetAllAsync();
        Task<Result<TickFormulaErrorCodes>> AddAsync(TickFormula model, string username);
        Task<Result<TickFormulaErrorCodes>> UpdateAsync(TickFormula model, string username);
        Task<Result<TickFormulaErrorCodes>> DeleteAsync(string id, string username);
    }
}