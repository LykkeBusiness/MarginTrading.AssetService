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
                Id = model.Id,
                RegulatoryTypeId = model.RegulatoryTypeId,
                UnderlyingCategoryId = model.UnderlyingCategoryId,
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
                        sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation)
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

                existingEntity.RegulatoryTypeId = model.RegulatoryTypeId;
                existingEntity.UnderlyingCategoryId = model.UnderlyingCategoryId;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException &&
                        sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation)
                    {
                        throw new AlreadyExistsException();
                    }

                    throw;
                }
            }
        }

        public async Task DeleteAsync(string id)
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
                        Id = r.Id,
                        RegulatoryTypeId = r.RegulatoryTypeId,
                        UnderlyingCategoryId = r.UnderlyingCategoryId,
                    })
                    .ToListAsync();

                return result;
            }
        }

        public async Task<IReadOnlyList<string>> GetAllIdsAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AssetTypes
                    .Select(r => r.Id)
                    .ToListAsync();

                return result;
            }
        }

        public async Task<AssetType> GetByIdAsync(string id)
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
                    UnderlyingCategoryId = entity.UnderlyingCategoryId,
                };
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AssetTypes.FindAsync(id);

                return result != null;
            }
        }        

        public async Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync(string regulatoryTypeId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AssetTypes.AnyAsync(x => x.RegulatoryTypeId == regulatoryTypeId);

                return result;
            }
        }

        public async Task<bool> AssignedToAnyProductAsync(string id)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var result = await context.Products.AnyAsync(x => x.AssetTypeId == id);

            return result;
        }
    }
}