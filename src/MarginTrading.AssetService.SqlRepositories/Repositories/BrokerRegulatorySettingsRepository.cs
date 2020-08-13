using System;
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
    public class BrokerRegulatorySettingsRepository : IBrokerRegulatorySettingsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public BrokerRegulatorySettingsRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task UpdateAsync(BrokerRegulatorySettings model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.BrokerRegulatorySettings.FindAsync(model.BrokerProfileId, model.BrokerTypeId);

                if (entity == null)
                    throw new BrokerRegulatorySettingsDoNotExistException();

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

        public async Task<BrokerRegulatorySettings> GetByIdsAsync(Guid profileId, Guid typeId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.BrokerRegulatorySettings
                    .Include(x => x.BrokerProfile)
                    .Include(x => x.BrokerType)
                    .FirstOrDefaultAsync(x => x.BrokerProfileId == profileId && x.BrokerTypeId == typeId);

                if (entity == null)
                    return null;

                return new BrokerRegulatorySettings
                {
                    BrokerProfileId = entity.BrokerProfileId,
                    BrokerTypeId = entity.BrokerTypeId,
                    MarginMin = entity.MarginMin,
                    IsAvailable = entity.IsAvailable,
                    ProfileName = entity.BrokerProfile.Name,
                    TypeName = entity.BrokerType.Name,
                    PhoneFees = entity.PhoneFees,
                    ExecutionFeesRate = entity.ExecutionFeesRate,
                    ExecutionFeesFloor = entity.ExecutionFeesFloor,
                    ExecutionFeesCap = entity.ExecutionFeesCap,
                    FinancingFeesRate = entity.FinancingFeesRate,
                };
            }
        }

        public async Task<IReadOnlyList<BrokerRegulatorySettings>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.BrokerRegulatorySettings
                    .Include(x => x.BrokerProfile)
                    .Include(x => x.BrokerType)
                    .Select(x => new BrokerRegulatorySettings
                {
                    BrokerProfileId = x.BrokerProfileId,
                    BrokerTypeId = x.BrokerTypeId,
                    MarginMin = x.MarginMin,
                    IsAvailable = x.IsAvailable,
                    ProfileName = x.BrokerProfile.Name,
                    TypeName = x.BrokerType.Name,
                    PhoneFees = x.PhoneFees,
                    ExecutionFeesRate = x.ExecutionFeesRate,
                    ExecutionFeesFloor = x.ExecutionFeesFloor,
                    ExecutionFeesCap = x.ExecutionFeesCap,
                    FinancingFeesRate = x.FinancingFeesRate,
                })
                    .ToListAsync();

                return result;
            }
        }
    }
}