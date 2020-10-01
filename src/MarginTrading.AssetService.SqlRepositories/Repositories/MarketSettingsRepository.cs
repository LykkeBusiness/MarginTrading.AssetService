// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class MarketSettingsRepository : IMarketSettingsRepository
    {
        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";
        private const int ForeignKeyConstraintViolation = 547;

        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public MarketSettingsRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MarketSettings> GetByIdAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existing = await context.MarketSettings
                    .FindAsync(id);

                if (existing == null)
                    return null;

                return ToDomain(existing);
            }
        }

        public async Task<IReadOnlyList<MarketSettings>> GetAllMarketSettingsAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.MarketSettings
                    .Select(x => ToDomain(x))
                    .ToListAsync();

                return result;
            }
        }

        public async Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettings model)
        {
            var entity = MarketSettingsEntity.Create(model);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.MarketSettings.Add(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<MarketSettingsErrorCodes>();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException)
                    {
                        switch (sqlException.Number)
                        {
                            case MsSqlErrorCodes.DuplicateIndex:
                                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.NameAlreadyExists);
                            case MsSqlErrorCodes.PrimaryKeyConstraintViolation:
                                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.IdAlreadyExists);
                        }
                    }

                    throw;
                }
            }
        }

        public async Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettings model)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existing = await context.MarketSettings.FindAsync(model.Id);

                if(existing == null)
                    return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);

                existing.MapFromDomain(model);
                context.MarketSettings.Update(existing);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<MarketSettingsErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.Message.Contains(DoesNotExistException))
                        return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);

                    throw;
                }
            }
        }

        public async Task<Result<MarketSettingsErrorCodes>> DeleteAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new MarketSettingsEntity { Id = id };

                context.Attach(entity);
                context.MarketSettings.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<MarketSettingsErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.Message.Contains(DoesNotExistException))
                        return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);

                    throw;
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException &&
                        sqlException.Number == ForeignKeyConstraintViolation)
                        return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.CannotDeleteMarketSettingsAssignedToAnyProduct);

                    throw;
                }
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.MarketSettings.AnyAsync(x => x.Id == id);

                return result;
            }
        }

        public async Task<bool> MarketSettingsAssignedToAnyProductAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.Products.AnyAsync(x => x.MarketId == id);

                return result;
            }
        }

        public async Task<IReadOnlyList<MarketSettings>> GetByIdsAsync(IEnumerable<string> ids)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.MarketSettings.AsNoTracking();

                if (ids != null && ids.Any())
                    query = query.Where(x => ids.Contains(x.Id));

                var result = await query
                    .Select(x => ToDomain(x))
                    .ToListAsync();

                return result;
            }
        }

        private static MarketSettings ToDomain(MarketSettingsEntity entity)
        {
            return new MarketSettings{
                Id = entity.Id,
                Name = entity.Name,
                DividendsLong = entity.DividendsLong,
                DividendsShort = entity.DividendsShort,
                Dividends871M = entity.Dividends871M,
                Close = entity.Close,
                Open = entity.Open,
                Timezone = entity.Timezone,
                Holidays = entity.Holidays.Select(x => x.Date).ToList(),
            };
        }
    }
}