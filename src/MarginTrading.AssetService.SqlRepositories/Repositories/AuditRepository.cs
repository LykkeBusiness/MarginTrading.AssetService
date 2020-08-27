using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public AuditRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task InsertAsync(IAuditModel model)
        {
            var entity = AuditEntity.Create(model);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.AuditTrail.Add(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<IAuditModel>> GetAll(int? year, int? month)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.AuditTrail
                    .AsNoTracking()
                    .Where(x =>
                        (!year.HasValue || x.Timestamp.Year == year) && (!month.HasValue || x.Timestamp.Month == month))
                    .ToListAsync();

                return result;
            }
        }
    }
}