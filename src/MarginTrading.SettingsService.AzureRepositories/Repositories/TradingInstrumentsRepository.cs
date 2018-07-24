using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class TradingInstrumentsRepository : GenericAzureCrudRepository<ITradingInstrument, TradingInstrumentEntity>,
        ITradingInstrumentsRepository
    {
        public TradingInstrumentsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "TradingInstruments")
        {

        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetByTradingConditionAsync(string tradingConditionId)
        {
            return (await TableStorage.GetDataAsync(x => x.TradingConditionId == tradingConditionId)).ToList();
        }

        public async Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null, 
            int? skip = null, int? take = null)
        {
            var allData = (string.IsNullOrWhiteSpace(tradingConditionId)
                ? await TableStorage.GetDataAsync()
                : await TableStorage.GetDataAsync(x => x.TradingConditionId == tradingConditionId)).ToList();

            //TODO refactor before using azure impl
            var data = allData.OrderBy(x => x.Id).ToList();
            var filtered = take.HasValue ? data.Skip(skip ?? 0).Take(PaginationHelper.GetTake(take)).ToList() : data;
            
            return new PaginatedResponse<ITradingInstrument>(
                contents: filtered,
                start: skip ?? 0,
                size: filtered.Count,
                totalSize: data.Count
            );
        }

        public async Task<IEnumerable<ITradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId, 
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults)
        {
            var objectsToAdd = assetPairsIds.Select(x => new TradingInstrument
            (
                tradingConditionId,
                x,
                defaults.LeverageInit,
                defaults.LeverageMaintenance,
                defaults.SwapLong,
                defaults.SwapShort,
                defaults.Delta,
                defaults.DealMinLimit,
                defaults.DealMaxLimit,
                defaults.PositionLimit,
                defaults.CommissionRate,
                defaults.CommissionMin,
                defaults.CommissionMax,
                defaults.CommissionCurrency
            )).ToList();
            var entitiesToAdd = objectsToAdd.Select(x =>
            {
                var entity = ConvertService.Convert(x, DefaultAzureMappingOpts);
                entity.SetKeys();
                return entity;
            });

            await TableStorage.InsertAsync(entitiesToAdd);

            return objectsToAdd;
        }

        public async Task UpdateAsync(ITradingInstrument tradingInstrument)
        {
            await base.ReplaceAsync(tradingInstrument);
        }

        public new async Task DeleteAsync(string assetPairId, string tradingConditionId)
        {
            await base.DeleteAsync(assetPairId, tradingConditionId);
        }
    }
}