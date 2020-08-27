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
    public class AssetTypesRepository : IAssetTypesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public AssetTypesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task InsertAsync(AssetTypeWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd)
        {
            var entity = new AssetTypeEntity
            {
                Name = model.Name,
                Id = model.Id,
                RegulatoryTypeId = model.RegulatoryTypeId,
                NormalizedName = model.Name.ToLower(),
            };

            var clientProfileSettingsEntities = clientProfileSettingsToAdd.Select(ClientProfileSettingsEntity.Create).ToArray();


            using (var context = _contextFactory.CreateDataContext())
            {
                context.AssetTypes.Add(entity);

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

        public async Task UpdateAsync(AssetType model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existingEntity = await context.AssetTypes.FindAsync(model.Id);

                if (existingEntity == null)
                    throw new AssetTypeDoesNotExistException();

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
                var existingEntity = await context.AssetTypes.FindAsync(id);

                if (existingEntity == null)
                    throw new AssetTypeDoesNotExistException();

                context.AssetTypes.Remove(existingEntity);

                var regulatorySettingsToRemove = await context.ClientProfileSettings.Where(p => p.AssetTypeId == id).ToArrayAsync();

                context.ClientProfileSettings.RemoveRange(regulatorySettingsToRemove);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<AssetType>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AssetTypes
                    .Select(r => new AssetType
                    {
                        Name = r.Name,
                        Id = r.Id,
                        RegulatoryTypeId = r.RegulatoryTypeId
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<IReadOnlyList<Guid>> GetAllIdsAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AssetTypes
                    .Select(r => r.Id)
                    .ToListAsync();

                return result;
            }
        }

        public async Task<AssetType> GetByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.AssetTypes.FindAsync(id);

                if (entity == null)
                    return null;

                return new AssetType
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
                var result = await context.AssetTypes.FindAsync(id);

                return result != null;
            }
        }
    }
}