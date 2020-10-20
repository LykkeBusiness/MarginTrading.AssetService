using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface ICurrenciesRepository
    {
        Task<Result<CurrenciesErrorCodes>> InsertAsync(Currency currency);
        Task<Result<CurrenciesErrorCodes>> UpdateAsync(Currency currency);
        Task<Result<CurrenciesErrorCodes>> DeleteAsync(string id, byte[] timestamp);
        Task<Result<Currency, CurrenciesErrorCodes>> GetByIdAsync(string id);
        Task<Result<List<Currency>, CurrenciesErrorCodes>> GetAllAsync();
        Task<Result<List<Currency>, CurrenciesErrorCodes>> GetByPageAsync(int skip, int take);
        Task<bool> CurrencyHasProductsAsync(string id);
        Task<IReadOnlyList<Currency>> GetByIdsAsync(IEnumerable<string> ids);
    }
}