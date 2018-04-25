using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class TradingInstrumentsRepository : GenericAzureCrudRepository<TradingInstrument, TradingInstrumentEntity>, 
        ITradingInstrumentsRepository
    {
        public TradingInstrumentsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "TradingInstruments")
        {

        }

        public async Task<IEnumerable<TradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId, 
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults)
        {
            var objectsToAdd = assetPairsIds.Select(x => new TradingInstrument
            {
                TradingConditionId = tradingConditionId,
                Instrument = x,
                LeverageInit = defaults.LeverageInit,
                LeverageMaintenance = defaults.LeverageMaintenance,
                SwapLong = defaults.SwapLong,
                SwapShort = defaults.SwapShort,
                Delta = defaults.Delta,
                DealMinLimit = defaults.DealMinLimit,
                DealMaxLimit = defaults.DealMaxLimit,
                PositionLimit = defaults.PositionLimit,
                CommissionRate = defaults.CommissionRate,
                CommissionMin = defaults.CommissionMin,
                CommissionMax = defaults.CommissionMax,
                CommissionCurrency = defaults.CommissionCurrency,
            }).ToList();
            var entitiesToAdd = objectsToAdd.Select(x =>
            {
                var entity = ConvertService.Convert(x, DefaultAzureMappingOpts);
                entity.SetRowKey();
                return entity;
            });

            await TableStorage.InsertAsync(entitiesToAdd);

            return objectsToAdd;
        }
    }
}