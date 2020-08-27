using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ClientProfilesRepository : IClientProfilesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private readonly ILog _log;

        public ClientProfilesRepository(MsSqlContextFactory<AssetDbContext> contextFactory, ILog log)
        {
            _contextFactory = contextFactory;
            _log = log;
        }

        public async Task InsertAsync(ClientProfileWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd)
        {
            var entity = new ClientProfileEntity
            {
                Name = model.Name,
                Id = model.Id,
                RegulatoryProfileId = model.RegulatoryProfileId,
                IsDefault = model.IsDefault,
                NormalizedName = model.Name.ToLower(),
            };

            var clientProfileSettingsEntities = clientProfileSettingsToAdd.Select(ClientProfileSettingsEntity.Create).ToArray();

            using (var context = _contextFactory.CreateDataContext())
            {
                var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x => x.IsDefault);

                if (currentDefault == null)
                {
                    entity.IsDefault = true;
                }
                else if (model.IsDefault)
                {
                    currentDefault.IsDefault = false;
                    context.Update(currentDefault);
                }

                context.ClientProfiles.Add(entity);

                context.ClientProfileSettings.AddRange(clientProfileSettingsEntities);

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

        public async Task UpdateAsync(ClientProfile model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.ClientProfiles.FindAsync(model.Id);

                if (existingEntity == null)
                    throw new ClientProfileDoesNotExistException();

                existingEntity.Name = model.Name;
                existingEntity.IsDefault = model.IsDefault;
                existingEntity.NormalizedName = model.Name.ToLower();

                var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x =>
                    x.IsDefault && x.Id != model.Id);

                if (currentDefault == null)
                {
                    existingEntity.IsDefault = true;
                    _log.Warning($"Tried to update ClientProfile with Id {model.Id} IsAvailable to false but it was not updated cause there will be no default");
                }
                else if (model.IsDefault)
                {
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
                var existingEntity = await context.ClientProfiles.FindAsync(id);

                if (existingEntity == null)
                    throw new ClientProfileDoesNotExistException();

                context.ClientProfiles.Remove(existingEntity);

                var regulatorySettingsToRemove = await context.ClientProfileSettings.Where(p => p.ClientProfileId == id).ToArrayAsync();

                context.ClientProfileSettings.RemoveRange(regulatorySettingsToRemove);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<ClientProfile>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfiles
                    .Select(r => new ClientProfile
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

        public async Task<ClientProfile> GetByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfiles.FindAsync(id);

                if (entity == null)
                    return null;

                return new ClientProfile
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
                var result = await context.ClientProfiles.FindAsync(id);

                return result != null;
            }
        }
    }
}