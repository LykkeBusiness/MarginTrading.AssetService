using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class AssetPairsRepository : GenericAzureCrudRepository<IAssetPair, AssetPairEntity>, IAssetPairsRepository
    {
        private readonly IConvertService _convertService;
        
        public AssetPairsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "AssetPairs")
        {
            _convertService = convertService;
        }

        public async Task<IReadOnlyList<IAssetPair>> GetAsync(params string[] assetPairIds)
        {
            return (assetPairIds.Length == 0
                ? await TableStorage.GetDataAsync()
                : await TableStorage.GetDataAsync(AssetPairEntity.Pk, x => assetPairIds.Contains(x.Id))).ToList();
        }

        public async Task<IAssetPair> GetByBaseAssetPairAsync(string baseAssetPairId)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, x => x.BaseAssetId == baseAssetPairId))
                .FirstOrDefault();
        }

        public async Task<IAssetPair> GetByBaseAssetPairAndNotByIdAsync(string id, string baseAssetPairId)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, 
                    x => x.Id != id && x.BaseAssetId == baseAssetPairId))
                .FirstOrDefault();
        }

        public async Task<IReadOnlyList<IAssetPair>> GetByLeAndMeModeAsync(string legalEntity = null, 
            string matchingEngineMode = null)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, 
                    x => (string.IsNullOrWhiteSpace(legalEntity) || x.LegalEntity == legalEntity)
                        && (string.IsNullOrWhiteSpace(matchingEngineMode) || x.MatchingEngineMode == matchingEngineMode)))
                .ToList();
        }

        public async Task<PaginatedResponse<IAssetPair>> GetByLeAndMeModeByPagesAsync(string legalEntity = null, 
            string matchingEngineMode = null, int? skip = null, int? take = null)
        {
            var allData = await GetByLeAndMeModeAsync(legalEntity, matchingEngineMode);

            //TODO refactor before using azure impl
            var data = allData.OrderBy(x => x.Id).ToList();
            var filtered = take.HasValue ? data.Skip(skip ?? 0).Take(PaginationHelper.GetTake(take)).ToList() : data;
            
            return new PaginatedResponse<IAssetPair>(
                contents: filtered,
                start: skip ?? 0,
                size: filtered.Count,
                totalSize: data.Count
            );
        }

        public async Task<IAssetPair> GetByBaseQuoteAndLegalEntityAsync(string baseAssetId, string quoteAssetId, string legalEntity)
        {
            var result = await TableStorage.GetDataAsync(x => x.BaseAssetId == baseAssetId 
                                                              && x.QuoteAssetId == quoteAssetId
                                                              && x.LegalEntity == legalEntity);

            return result.FirstOrDefault();
        }

        public new async Task<IAssetPair> GetAsync(string assetPairId)
        {
            return await base.GetAsync(assetPairId, AssetPairEntity.Pk);
        }

        public async Task<bool> TryInsertBatchAsync(IReadOnlyList<IAssetPair> assetPairs)
        {
            var assetPairEntities = assetPairs.Select(x => _convertService.Convert<IAssetPair, AssetPairEntity>(
                ((AssetPair) x).CreateForUpdate(false))).ToList();
            
            //TODO batch insert is done in 2 transactions which is a point of inconsistency
            var existing = await TableStorage.GetDataAsync(AssetPairEntity.Pk, assetPairEntities.Select(x => x.RowKey));
            if (existing.Any())
            {
                return false;
            }

            await TableStorage.InsertOrMergeBatchAsync(assetPairEntities);
            return true;
        }

        public async Task DeleteAsync(string assetPairId)
        {
            await base.DeleteAsync(assetPairId);
        }

        public new async Task<bool> TryInsertAsync(IAssetPair obj)
        {
            var current = await TableStorage.GetDataAsync(AssetPairEntity.Pk, obj.Id);

            if (current != null)
            {
                throw new ArgumentException("Asset pair already exists", nameof(obj));
            }
            
            return await base.TryInsertAsync(((AssetPair)obj).CreateForUpdate(false));
        }

        public async Task UpdateAsync(IAssetPair obj)
        {
            var current = await TableStorage.GetDataAsync(AssetPairEntity.Pk, obj.Id);

            if (current == null)
            {
                throw new ArgumentException("Asset pair does not exist", nameof(obj));
            }
            
            await base.ReplaceAsync(((AssetPair)obj).CreateForUpdate(current.IsSuspended));
        }

        public async Task UpdateBatchAsync(IReadOnlyList<IAssetPair> assetPairs)
        {
            //TODO batch update is done in 2 transactions which is a point of inconsistency
            var existing = (await TableStorage.GetDataAsync(AssetPairEntity.Pk, assetPairs.Select(x => x.Id)))
                .ToDictionary(x => x.Id, x => x.IsSuspended);
            if (existing.Count != assetPairs.Count)
            {
                throw new ArgumentException("One of asset pairs does not exist", nameof(assetPairs));
            }
            
            var assetPairEntities = assetPairs.Select(x => _convertService.Convert<IAssetPair, AssetPairEntity>(
                ((AssetPair) x).CreateForUpdate(existing[x.Id]))).ToList();

            await TableStorage.InsertOrReplaceBatchAsync(assetPairEntities);
        }

        public async Task<IAssetPair> ChangeSuspendFlag(string assetPairId, bool suspendFlag)
        {
            IAssetPair current = await TableStorage.GetDataAsync(AssetPairEntity.Pk, assetPairId);
            
            if (current == null)
            {
                throw new ArgumentException("Asset pair does not exist", nameof(assetPairId));
            }

            var newAssetPair = ((AssetPair) current).CreateForUpdate(suspendFlag);
            
            await base.ReplaceAsync(newAssetPair);

            return newAssetPair;
        }
    }
}