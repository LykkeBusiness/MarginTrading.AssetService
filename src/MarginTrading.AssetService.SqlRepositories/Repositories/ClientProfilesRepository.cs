using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ClientProfilesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task InsertAsync(ClientProfileWithTemplate model, TransactionContext txContext = null)
        {
            var entity = new ClientProfileEntity
            {
                Name = model.Name,
                Id = model.Id,
                RegulatoryProfileId = model.RegulatoryProfileId,
                IsDefault = model.IsDefault,
                NormalizedName = model.Name.ToLower(),
            };

            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                context.ClientProfiles.Add(entity);

                if (model.IsDefault)
                {
                    var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x => x.IsDefault);
                    if (currentDefault != null)
                    {
                        currentDefault.IsDefault = false;
                        context.Update(currentDefault);
                    }
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

        public async Task UpdateAsync(ClientProfile model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.ClientProfiles.FindAsync(model.Id);

                if (existingEntity == null)
                    throw new ClientProfileDoesNotExistException();

                existingEntity.Name = model.Name;
                existingEntity.NormalizedName = model.Name.ToLower();

                if (model.IsDefault)
                {
                    var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x => x.IsDefault);
                    if (currentDefault != null)
                    {
                        currentDefault.IsDefault = false;
                        context.Update(currentDefault);
                    }
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