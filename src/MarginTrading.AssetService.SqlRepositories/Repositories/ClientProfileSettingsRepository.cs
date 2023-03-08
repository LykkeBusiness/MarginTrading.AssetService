using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ClientProfileSettingsRepository : IClientProfileSettingsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public ClientProfileSettingsRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task UpdateAsync(ClientProfileSettings model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfileSettings.FindAsync(model.ClientProfileId, model.AssetTypeId);

                if (entity == null)
                    throw new ClientSettingsDoNotExistException();

                entity.Margin = model.Margin;
                entity.IsAvailable = model.IsAvailable;
                entity.ExecutionFeesRate = model.ExecutionFeesRate;
                entity.ExecutionFeesCap = model.ExecutionFeesCap;
                entity.ExecutionFeesFloor = model.ExecutionFeesFloor;
                entity.FinancingFeesRate = model.FinancingFeesRate;
                entity.OnBehalfFee = model.OnBehalfFee;

                context.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<ClientProfileSettings> GetByIdsAsync(string profileId, string typeId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfileSettings
                    .Include(x => x.ClientProfile)
                    .Include(x => x.AssetType)
                    .FirstOrDefaultAsync(x => x.ClientProfileId == profileId && x.AssetTypeId == typeId);

                if (entity == null)
                    return null;

                return new ClientProfileSettings
                {
                    ClientProfileId = entity.ClientProfileId,
                    AssetTypeId = entity.AssetTypeId,
                    Margin = entity.Margin,
                    IsAvailable = entity.IsAvailable,
                    OnBehalfFee = entity.OnBehalfFee,
                    ExecutionFeesRate = entity.ExecutionFeesRate,
                    ExecutionFeesFloor = entity.ExecutionFeesFloor,
                    ExecutionFeesCap = entity.ExecutionFeesCap,
                    FinancingFeesRate = entity.FinancingFeesRate,
                    RegulatoryProfileId = entity.ClientProfile.RegulatoryProfileId,
                    RegulatoryTypeId = entity.AssetType.RegulatoryTypeId,
                    ExcludeSpreadFromProductCosts = entity.AssetType.ExcludeSpreadFromProductCosts
                };
            }
        }

        public async Task<List<ClientProfileSettings>> GetAllAsync(string clientProfileId, string assetTypeId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.ClientProfileSettings.AsNoTracking();

                if (!string.IsNullOrEmpty(clientProfileId))
                    query = query.Where(x => x.ClientProfileId == clientProfileId);

                if (!string.IsNullOrEmpty(assetTypeId))
                    query = query.Where(x => x.AssetTypeId == assetTypeId);

                var result = await query
                    .Include(x => x.ClientProfile)
                    .Include(x => x.AssetType)
                    .Select(x => new ClientProfileSettings
                    {
                        ClientProfileId = x.ClientProfileId,
                        AssetTypeId = x.AssetTypeId,
                        Margin = x.Margin,
                        IsAvailable = x.IsAvailable,
                        OnBehalfFee = x.OnBehalfFee,
                        ExecutionFeesRate = x.ExecutionFeesRate,
                        ExecutionFeesFloor = x.ExecutionFeesFloor,
                        ExecutionFeesCap = x.ExecutionFeesCap,
                        FinancingFeesRate = x.FinancingFeesRate,
                        RegulatoryProfileId = x.ClientProfile.RegulatoryProfileId,
                        RegulatoryTypeId = x.AssetType.RegulatoryTypeId,
                        ExcludeSpreadFromProductCosts = x.AssetType.ExcludeSpreadFromProductCosts
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<List<ClientProfileSettings>> GetAllAsync(string clientProfileId, 
            IEnumerable<string> assetTypeIds, 
            bool availableOnly)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfileSettings
                    .Where(x => string.IsNullOrEmpty(clientProfileId) || x.ClientProfileId == clientProfileId)
                    .Where(x => assetTypeIds.Contains(x.AssetTypeId))
                    .Where(x => !availableOnly || x.IsAvailable)
                    .Include(x => x.ClientProfile)
                    .Include(x => x.AssetType)
                    .Select(x => new ClientProfileSettings
                    {
                        ClientProfileId = x.ClientProfileId,
                        AssetTypeId = x.AssetTypeId,
                        Margin = x.Margin,
                        IsAvailable = x.IsAvailable,
                        OnBehalfFee = x.OnBehalfFee,
                        ExecutionFeesRate = x.ExecutionFeesRate,
                        ExecutionFeesFloor = x.ExecutionFeesFloor,
                        ExecutionFeesCap = x.ExecutionFeesCap,
                        FinancingFeesRate = x.FinancingFeesRate,
                        RegulatoryProfileId = x.ClientProfile.RegulatoryProfileId,
                        RegulatoryTypeId = x.AssetType.RegulatoryTypeId,
                        ExcludeSpreadFromProductCosts = x.AssetType.ExcludeSpreadFromProductCosts
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<bool> WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(RegulatorySettingsDto regulatorySettings)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfileSettings
                    .Include(x => x.AssetType)
                    .Include(x => x.ClientProfile)
                    .AnyAsync(x => x.ClientProfile.RegulatoryProfileId == regulatorySettings.RegulatoryProfileId &&
                                   x.AssetType.RegulatoryTypeId == regulatorySettings.RegulatoryTypeId &&
                                   (x.Margin < regulatorySettings.MarginMin ||
                                   x.IsAvailable && !regulatorySettings.IsAvailable));

                return result;
            }
        }
        
        public async Task<List<string>> GetActiveAssetTypeIdsAsync(bool defaultProfileOnly)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfileSettings
                    .Include(x => x.ClientProfile)
                    .Where(x => x.IsAvailable && (!defaultProfileOnly || x.ClientProfile.IsDefault))
                    .Select(x => x.AssetTypeId)
                    .ToListAsync();

                return result;
            }
        }

        public async Task<List<string>> GetActiveAssetTypeIdsAsync(string clientProfileId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfileSettings
                    .Include(x => x.ClientProfile)
                    .Where(x => x.IsAvailable &&
                                (string.IsNullOrEmpty(clientProfileId) || x.ClientProfile.Id == clientProfileId))
                    .Select(x => x.AssetTypeId)
                    .ToListAsync();

                return result;
            }
        }
    }
}