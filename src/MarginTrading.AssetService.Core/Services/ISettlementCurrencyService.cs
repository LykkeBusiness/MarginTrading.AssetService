using System.Threading.Tasks;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ISettlementCurrencyService
    {
        Task<string> GetSettlementCurrencyAsync();
    }
}