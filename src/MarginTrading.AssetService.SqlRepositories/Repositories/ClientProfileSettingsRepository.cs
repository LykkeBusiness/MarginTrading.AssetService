using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.SqlRepositories.Entities;
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

        public async Task InsertMultipleAsync(IEnumerable<ClientProfileSettings> settings, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entities = settings.Select(ClientProfileSettingsEntity.Create).ToArray();

                context.ClientProfileSettings.AddRange(entities);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(ClientProfileSettings model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfileSettings.FindAsync(model.ClientProfileId, model.AssetTypeId);

                if (entity == null)
                    throw new ClientSettingsDoNotExistException();

                entity.MarginMin = model.MarginMin;
                entity.IsAvailable = model.IsAvailable;
                entity.ExecutionFeesRate = model.ExecutionFeesRate;
                entity.ExecutionFeesCap = model.ExecutionFeesCap;
                entity.ExecutionFeesFloor = model.ExecutionFeesFloor;
                entity.FinancingFeesRate = model.FinancingFeesRate;
                entity.PhoneFees = model.PhoneFees;

                context.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<ClientProfileSettings> GetByIdsAsync(Guid profileId, Guid typeId)
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
                    MarginMin = entity.MarginMin,
                    IsAvailable = entity.IsAvailable,
                    ClientProfileName = entity.ClientProfile.Name,
                    AssetTypeName = entity.AssetType.Name,
                    PhoneFees = entity.PhoneFees,
                    ExecutionFeesRate = entity.ExecutionFeesRate,
                    ExecutionFeesFloor = entity.ExecutionFeesFloor,
                    ExecutionFeesCap = entity.ExecutionFeesCap,
                    FinancingFeesRate = entity.FinancingFeesRate,
                    RegulatoryProfileId = entity.ClientProfile.RegulatoryProfileId,
                    RegulatoryTypeId = entity.AssetType.RegulatoryTypeId,
                };
            }
        }

        public async Task<List<ClientProfileSettings>> GetAllAsync(Guid? clientProfileId, Guid? assetTypeId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.ClientProfileSettings.AsNoTracking();

                if (clientProfileId.HasValue)
                    query = query.Where(x => x.ClientProfileId == clientProfileId.Value);

                if (assetTypeId.HasValue)
                    query = query.Where(x => x.AssetTypeId == assetTypeId.Value);

                var result = await query
                    .Include(x => x.ClientProfile)
                    .Include(x => x.AssetType)
                    .Select(x => new ClientProfileSettings
                    {
                        ClientProfileId = x.ClientProfileId,
                        AssetTypeId = x.AssetTypeId,
                        MarginMin = x.MarginMin,
                        IsAvailable = x.IsAvailable,
                        ClientProfileName = x.ClientProfile.Name,
                        AssetTypeName = x.AssetType.Name,
                        PhoneFees = x.PhoneFees,
                        ExecutionFeesRate = x.ExecutionFeesRate,
                        ExecutionFeesFloor = x.ExecutionFeesFloor,
                        ExecutionFeesCap = x.ExecutionFeesCap,
                        FinancingFeesRate = x.FinancingFeesRate,
                        RegulatoryProfileId = x.ClientProfile.RegulatoryProfileId,
                        RegulatoryTypeId = x.AssetType.RegulatoryTypeId,
                    })
                    .ToListAsync();

                return result;
            }
        }
    }
}