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
    public class BrokerRegulatoryProfilesRepository : IBrokerRegulatoryProfilesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public BrokerRegulatoryProfilesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task InsertAsync(BrokerRegulatoryProfileWithTemplate model)
        {
            var entity = new BrokerRegulatoryProfileEntity
            {
                Name = model.Name,
                Id = model.Id,
                RegulatoryProfileId = model.RegulatoryProfileId,
                IsDefault = model.IsDefault,
                NormalizedName = model.Name.ToLower(),
            };

            using (var context = _contextFactory.CreateDataContext())
            {
                context.BrokerRegulatoryProfiles.Add(entity);

                if (model.IsDefault)
                {
                    var currentDefault = await context.BrokerRegulatoryProfiles.FirstOrDefaultAsync(x => x.IsDefault);
                    if (currentDefault != null)
                    {
                        currentDefault.IsDefault = false;
                        context.Update(currentDefault);
                    }
                }

                var regulatorySettings = new List<BrokerRegulatorySettingsEntity>();

                if (model.BrokerRegulatoryProfileTemplateId.HasValue)
                {
                    regulatorySettings = await context.BrokerRegulatorySettings
                        .Where(x => x.BrokerProfileId == model.BrokerRegulatoryProfileTemplateId.Value).AsNoTracking().ToListAsync();

                    foreach (var settingsEntity in regulatorySettings)
                    {
                        settingsEntity.BrokerProfileId = model.Id;
                        //If Min margin value from MDM is higher than in the current settings set it as min
                        settingsEntity.MarginMin = settingsEntity.MarginMin <= model.MinMargin ? model.MinMargin : settingsEntity.MarginMin;
                        //Is available only if regulatory settings from MDM is available and current one is available
                        settingsEntity.IsAvailable = model.IsAvailable && settingsEntity.IsAvailable;
                    }
                }
                else
                {
                    var regulatoryTypesIds = await context.BrokerRegulatoryTypes
                        .Select(x => x.Id)
                        .ToArrayAsync();

                    foreach (var regulatoryTypesId in regulatoryTypesIds)
                    {
                        regulatorySettings.Add(new BrokerRegulatorySettingsEntity
                        {
                            BrokerTypeId = regulatoryTypesId,
                            BrokerProfileId = model.Id,
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

        public async Task UpdateAsync(BrokerRegulatoryProfile model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.BrokerRegulatoryProfiles.FindAsync(model.Id);

                if (existingEntity == null)
                    throw new RegulatoryProfileDoesNotExistException();

                existingEntity.Name = model.Name;
                existingEntity.NormalizedName = model.Name.ToLower();

                if (model.IsDefault)
                {
                    var currentDefault = await context.BrokerRegulatoryProfiles.FirstOrDefaultAsync(x => x.IsDefault);
                    currentDefault.IsDefault = false;
                    context.Update(currentDefault);
                }

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
                var existingEntity = await context.BrokerRegulatoryProfiles.FindAsync(id);

                if (existingEntity == null)
                    throw new RegulatoryProfileDoesNotExistException();

                context.BrokerRegulatoryProfiles.Remove(existingEntity);

                var regulatorySettingsToRemove = await context.BrokerRegulatorySettings.Where(p => p.BrokerProfileId == id).ToArrayAsync();

                context.BrokerRegulatorySettings.RemoveRange(regulatorySettingsToRemove);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<BrokerRegulatoryProfile>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.BrokerRegulatoryProfiles
                    .Select(r => new BrokerRegulatoryProfile
                    {
                        Name = r.Name,
                        Id = r.Id,
                        IsDefault = r.IsDefault,
                        RegulatoryProfileId = r.RegulatoryProfileId,
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<BrokerRegulatoryProfile> GetByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.BrokerRegulatoryProfiles.FindAsync(id);

                if (entity == null)
                    return null;

                return new BrokerRegulatoryProfile
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    IsDefault = entity.IsDefault,
                    RegulatoryProfileId = entity.RegulatoryProfileId,
                };
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.BrokerRegulatoryProfiles.FindAsync(id);

                return result != null;
            }
        }
    }
}