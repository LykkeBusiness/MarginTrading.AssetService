using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Client.TradingConditions;
using Refit;

namespace MarginTrading.SettingsService.Client
{
    [PublicAPI]
    public interface ITradingConditionsApi
    {
        [Get("/api/tradingConditions")]
        Task<List<TradingConditionContract>> List();


        [Post("/api/tradingConditions")]
        Task<TradingConditionContract> Insert(
            [Body] TradingConditionContract tradingCondition);


        [Get("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Get(
            [NotNull] string tradingConditionId);

        
        [Put("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Update(
            [NotNull] string tradingConditionId,
            [Body] TradingConditionContract tradingCondition);

    }
}
