using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class BrokerRegulatoryTypesRepository : IBrokerRegulatoryTypesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public BrokerRegulatoryTypesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task InsertAsync(BrokerRegulatoryTypeWithTemplate model)
        {
            var entity = new BrokerRegulatoryTypeEntity
            {
                Name = model.Name,
                Id = model.Id,
                RegulatoryTypeId = model.RegulatoryTypeId,
                NormalizedName = model.Name.ToLower(),
            };

            using (var context = _contextFactory.CreateDataContext())
            {
                context.BrokerRegulatoryTypes.Add(entity);

                var regulatorySettings = new List<BrokerRegulatorySettingsEntity>();

                if (model.BrokerRegulatoryTypeTemplateId.HasValue)
                {
                    regulatorySettings = await context.BrokerRegulatorySettings
                        .Where(x => x.BrokerTypeId == model.BrokerRegulatoryTypeTemplateId.Value).AsNoTracking().ToListAsync();

                    foreach (var settingsEntity in regulatorySettings)
                    {
                        settingsEntity.BrokerTypeId = model.Id;
                        //If Min margin value from MDM is higher than in the current settings set it as min
                        settingsEntity.MarginMin = settingsEntity.MarginMin <= model.MinMargin ? model.MinMargin : settingsEntity.MarginMin;
                        //Is available only if regulatory settings from MDM is available and current one is available
                        settingsEntity.IsAvailable = model.IsAvailable && settingsEntity.IsAvailable;
                    }
                }
                else
                {
                    var regulatoryProfilesIds = await context.BrokerRegulatoryProfiles
                        .Select(x => x.Id)
                        .ToArrayAsync();

                    foreach (var regulatoryProfileId in regulatoryProfilesIds)
                    {
                        regulatorySettings.Add(new BrokerRegulatorySettingsEntity
                        {
                            BrokerTypeId = model.Id,
                            BrokerProfileId = regulatoryProfileId,
                            MarginMin = model.MinMargin,
                            IsAvailable = model.IsAvailable,
                        });
                    }
                }

                context.BrokerRegulatorySettings.AddRange(regulatorySettings);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException &&
                        sqlException.Number == MsSqlErrorCodes.DuplicateIndex)
                    {
                        throw new AlreadyExistsException();
                    }

                    throw;
                }
            }
        }

        public async Task UpdateAsync(BrokerRegulatoryType model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.BrokerRegulatoryTypes.FindAsync(model.Id);

                if (existingEntity == null)
                    throw new RegulatoryTypeDoesNotExistException();

                existingEntity.Name = model.Name;
                existingEntity.NormalizedName = model.Name.ToLower();

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException &&
                        sqlException.Number == MsSqlErrorCodes.DuplicateIndex)
                    {
                        throw new AlreadyExistsException();
                    }

                    throw;
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.BrokerRegulatoryTypes.FindAsync(id);

                if (existingEntity == null)
                    throw new RegulatoryTypeDoesNotExistException();

                context.BrokerRegulatoryTypes.Remove(existingEntity);

                var regulatorySettingsToRemove = await context.BrokerRegulatorySettings.Where(p => p.BrokerTypeId == id).ToArrayAsync();

                context.BrokerRegulatorySettings.RemoveRange(regulatorySettingsToRemove);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<BrokerRegulatoryType>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.BrokerRegulatoryTypes
                    .Select(r => new BrokerRegulatoryType
                    {
                        Name = r.Name,
                        Id = r.Id,
                        RegulatoryTypeId = r.RegulatoryTypeId
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<BrokerRegulatoryType> GetByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.BrokerRegulatoryTypes.FindAsync(id);

                if (entity == null)
                    return null;

                return new BrokerRegulatoryType
                {
                    Id = entity.Id,
                    RegulatoryTypeId = entity.RegulatoryTypeId,
                    Name = entity.Name,
                };
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.BrokerRegulatoryTypes.FindAsync(id);

                return result != null;
            }
        }
    }
}