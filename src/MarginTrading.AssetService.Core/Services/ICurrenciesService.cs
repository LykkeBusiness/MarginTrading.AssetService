using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ICurrenciesService
    {
        Task<Result<CurrenciesErrorCodes>> InsertAsync(Currency currency, string username);
        Task<Result<CurrenciesErrorCodes>> UpdateAsync(Currency currency, string username);
        Task<Result<CurrenciesErrorCodes>> DeleteAsync(string id, string username);
        Task<Result<Currency, CurrenciesErrorCodes>> GetByIdAsync(string id);
        Task<Result<List<Currency>, CurrenciesErrorCodes>> GetByPageAsync(int skip = default, int take = 20);
        Task<Result<List<Currency>, CurrenciesErrorCodes>> GetAllAsync();
    }
}