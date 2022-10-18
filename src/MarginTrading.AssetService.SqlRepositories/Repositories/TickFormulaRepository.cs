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
    public class TickFormulaRepository : ITickFormulaRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public TickFormulaRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ITickFormula> GetByIdAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var existing = await context.TickFormulas
                    .FindAsync(id);

                return existing;
            }
        }

        public async Task<IReadOnlyList<ITickFormula>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.TickFormulas
                    .ToListAsync();

                return result;
            }
        }

        public async Task<Result<TickFormulaErrorCodes>> AddAsync(ITickFormula model)
        {
            var entity = TickFormulaEntity.Create(model);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.TickFormulas.Add(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<TickFormulaErrorCodes>();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlException && sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation)
                        return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.AlreadyExist);

                    throw;
                }
            }
        }

        public async Task<Result<TickFormulaErrorCodes>> UpdateAsync(ITickFormula model)
        {
            var entity = TickFormulaEntity.Create(model);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.TickFormulas.Update(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<TickFormulaErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
                {
                    return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.TickFormulaDoesNotExist);
                }
            }
        }

        public async Task<Result<TickFormulaErrorCodes>> DeleteAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new TickFormulaEntity { Id = id };

                context.Attach(entity);
                context.TickFormulas.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<TickFormulaErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
                {
                    return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.TickFormulaDoesNotExist);
                }
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var result = await context.TickFormulas.AnyAsync(x => x.Id == id);

            return result;
        }
        
        public async Task<bool> AssignedToAnyProductAsync(string id)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var result = await context.Products.AnyAsync(x => x.TickFormulaId == id);

            return result;
        }

        public async Task<IReadOnlyList<ITickFormula>> GetByIdsAsync(IEnumerable<string> ids)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.TickFormulas.AsNoTracking();

                if (ids != null && ids.Any())
                    query = query.Where(x => ids.Contains(x.Id));

                return await query.ToListAsync();
            }
        }
    }
}