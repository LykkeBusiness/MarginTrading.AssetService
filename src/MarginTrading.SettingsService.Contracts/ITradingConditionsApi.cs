using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
    [PublicAPI]
    public interface ITradingConditionsApi
    {
        [Get("/api/tradingConditions")]
        Task<List<TradingConditionContract>> List();


        [Post("/api/tradingConditions")]
        Task<TradingConditionContract> Insert([Body] TradingConditionContract tradingCondition);


        [ItemCanBeNull]
        [Get("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Get([NotNull] string tradingConditionId);

        
        [ItemCanBeNull]
        [Get("/api/tradingConditions/default")]
        Task<TradingConditionContract> GetDefault();
        
        
        [Put("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Update(
            [NotNull] string tradingConditionId,
            [Body] TradingConditionContract tradingCondition);

    }
}
