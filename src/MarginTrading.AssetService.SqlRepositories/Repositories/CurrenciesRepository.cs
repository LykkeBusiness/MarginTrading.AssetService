using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class CurrenciesRepository : ICurrenciesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public CurrenciesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<CurrenciesErrorCodes>> InsertAsync(Currency currency)
        {
            var entity = ToEntity(currency);
            using (var context = _contextFactory.CreateDataContext())
            {
                await context.Currencies.AddAsync(entity);
                
                try
                {
                    await context.SaveChangesAsync();
                    return new Result<CurrenciesErrorCodes>();
                }
                catch (DbUpdateException e)
                {
                    if (e.ValueAlreadyExistsException())
                    {
                        return new Result<CurrenciesErrorCodes>(CurrenciesErrorCodes.AlreadyExists);
                    }

                    throw;
                }
            }
        }

        public async Task<Result<CurrenciesErrorCodes>> UpdateAsync(Currency currency)
        {
            var entity = ToEntity(currency);
            using (var context = _contextFactory.CreateDataContext())
            {
                context.Update(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<CurrenciesErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if(e.Message.Contains(DoesNotExistException)) 
                        return new Result<CurrenciesErrorCodes>(CurrenciesErrorCodes.DoesNotExist);

                    throw;
                }
            }
        }
        
        public async Task<Result<CurrenciesErrorCodes>> DeleteAsync(string id, byte[] timestamp)
        {
            var entity = new CurrencyEntity() {Id = id, Timestamp = timestamp};
            
            using (var context = _contextFactory.CreateDataContext())
            {
                context.Attach(entity);
                context.Currencies.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<CurrenciesErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if(e.Message.Contains(DoesNotExistException)) 
                        return new Result<CurrenciesErrorCodes>(CurrenciesErrorCodes.DoesNotExist);
                    
                    throw;
                }
            }
        }
        
        public async Task<Result<Currency, CurrenciesErrorCodes>> GetByIdAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.Currencies.FindAsync(id);

                if (entity == null)
                    return new Result<Currency, CurrenciesErrorCodes>(CurrenciesErrorCodes.DoesNotExist);

                var model = ToModel(entity);
                
                return new Result<Currency, CurrenciesErrorCodes>(model);
            }
        }

        public async Task<Result<List<Currency>, CurrenciesErrorCodes>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Currencies
                    .OrderBy(u => u.Id)
                    .ToListAsync();

                var result = entities.Select(ToModel).ToList();
                
                return new Result<List<Currency>, CurrenciesErrorCodes>(result);
            }
        }
        
        public async Task<Result<List<Currency>, CurrenciesErrorCodes>> GetByPageAsync(int skip, int take)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Currencies
                    .OrderBy(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
        
                var result = entities.Select(ToModel).ToList();
                return new Result<List<Currency>, CurrenciesErrorCodes>(result);
            }
        }

        public async Task<bool> CurrencyHasProductsAsync(string id)
        {
            await using var context = _contextFactory.CreateDataContext();

            return await context.Products.AnyAsync(p => p.TradingCurrencyId == id);

        }

        private CurrencyEntity ToEntity(Currency currency)
        {
            return new CurrencyEntity()
            {
                // required but with default = 2, FE will not change it right now, it is required for backward compatibility with existing model
                Accuracy = 2,
                Id = currency.Id,
                InterestRateMdsCode = currency.InterestRateMdsCode,
                Timestamp = currency.Timestamp,
            };
        }
        
        private Currency ToModel(CurrencyEntity entity)
        {
            return new  Currency()
            {
                Accuracy = entity.Accuracy,
                Id = entity.Id,
                Timestamp = entity.Timestamp,
                InterestRateMdsCode = entity.InterestRateMdsCode,
            };
        }
    }
}