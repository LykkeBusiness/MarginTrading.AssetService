using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ClientProfilesRepository : IClientProfilesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private readonly ILogger<ClientProfilesRepository> _logger;

        public ClientProfilesRepository(MsSqlContextFactory<AssetDbContext> contextFactory, ILogger<ClientProfilesRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        [ItemCanBeNull]
        public async Task<ClientProfile> InsertAsync(ClientProfileWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd)
        {
            var entity = new ClientProfileEntity
            {
                Id = model.Id,
                RegulatoryProfileId = model.RegulatoryProfileId,
                IsDefault = model.IsDefault
            };

            var clientProfileSettingsEntities = clientProfileSettingsToAdd.Select(ClientProfileSettingsEntity.Create).ToArray();

            await using var context = _contextFactory.CreateDataContext();
            var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x => x.IsDefault);
            var defaultProfileChanged = false;
            
            if(currentDefault?.Id == model.Id)
                throw new AlreadyExistsException();
            
            if (currentDefault == null)
            {
                entity.IsDefault = true;
            }
            else if (model.IsDefault)
            {
                currentDefault.IsDefault = false;
                context.Update(currentDefault);
                defaultProfileChanged = true;
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
                    sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation)
                {
                    throw new AlreadyExistsException();
                }
            
                throw;
            }

            return defaultProfileChanged
                ? new ClientProfile(currentDefault.Id, currentDefault.RegulatoryProfileId, currentDefault.IsDefault)
                : null;
        }

        public async Task<ClientProfile> UpdateAsync(ClientProfile model)
        {
            await using var context = _contextFactory.CreateDataContext();
            var existingEntity = await context.ClientProfiles.FindAsync(model.Id);

            if (existingEntity == null)
                throw new ClientProfileDoesNotExistException();

            existingEntity.IsDefault = model.IsDefault;
            existingEntity.RegulatoryProfileId = model.RegulatoryProfileId;

            var currentDefault = await context.ClientProfiles.FirstOrDefaultAsync(x =>
                x.IsDefault && x.Id != model.Id);
            var defaultProfileChanged = false;

            if (currentDefault == null)
            {
                existingEntity.IsDefault = true;
                _logger.LogWarning("Tried to update ClientProfile with Id {ClientProfileId} IsAvailable to false but it was not updated cause there will be no default", model.Id);
            }
            else if (model.IsDefault)
            {
                currentDefault.IsDefault = false;
                context.Update(currentDefault);
                defaultProfileChanged = true;
            }

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
            
            return defaultProfileChanged
                ? new ClientProfile(currentDefault.Id, currentDefault.RegulatoryProfileId, currentDefault.IsDefault)
                : null;
        }

        public async Task DeleteAsync(string id)
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
                    .Select(r => new ClientProfile(r.Id, r.RegulatoryProfileId, r.IsDefault))
                    .ToListAsync();

                return result;
            }
        }

        public async Task<ClientProfile> GetByIdAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfiles.FindAsync(id);

                if (entity == null)
                    return null;

                return new ClientProfile(entity.Id, entity.RegulatoryProfileId, entity.IsDefault);
            }
        }

        public async Task<ClientProfile> GetDefaultAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ClientProfiles.FirstOrDefaultAsync(x => x.IsDefault);

                if (entity == null)
                    return null;

                return new ClientProfile(entity.Id, entity.RegulatoryProfileId, entity.IsDefault);
            }
        }

        public async Task<IReadOnlyList<ClientProfile>> GetByDefaultFilterAsync(bool isDefault)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfiles
                    .Where(x => x.IsDefault == isDefault)
                    .Select(r => new ClientProfile(r.Id, r.RegulatoryProfileId, r.IsDefault))
                    .ToListAsync();

                return result;
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfiles.FindAsync(id);

                return result != null;
            }
        }

        public async Task<bool> IsRegulatoryProfileAssignedToAnyClientProfileAsync(string regulatoryProfileId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.ClientProfiles.AnyAsync(x => x.RegulatoryProfileId == regulatoryProfileId);

                return result;
            }
        }
    }
}