using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common;
using MarginTrading.AssetService.Core.Domain;
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

        public async Task InsertAsync(IAuditModel<AuditDataType> model)
        {
            var entity = AuditEntity.Create(model);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.AuditTrail.Add(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int skip, int take)
        {
            await using var context = _contextFactory.CreateDataContext();
            var query = context.AuditTrail.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.UserName))
                query = query.Where(x => x.UserName.ToLower().Contains(filter.UserName.ToLower()));

            if (!string.IsNullOrEmpty(filter.CorrelationId))
                query = query.Where(x => x.CorrelationId == filter.CorrelationId);

            if (!string.IsNullOrEmpty(filter.ReferenceId))
                query = query.Where(x => x.DataReference.ToLower().Contains(filter.ReferenceId.ToLower()));

            if (filter.DataTypes != null && filter.DataTypes.Any())
                query = query.Where(x => filter.DataTypes.Contains(x.DataType));

            if (filter.ActionType.HasValue)
                query = query.Where(x => x.Type == filter.ActionType.Value);

            if (filter.StartDateTime.HasValue)
                query = query.Where(x => x.Timestamp >= filter.StartDateTime.Value);

            if (filter.EndDateTime.HasValue)
                query = query.Where(x => x.Timestamp <= filter.EndDateTime.Value);

            var total = await query.CountAsync();

            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            query = query
                .OrderByDescending(x => x.Timestamp)
                .Skip(skip)
                .Take(take);
            
            var contents = await query.ToListAsync();

            var result = new PaginatedResponse<IAuditModel<AuditDataType>>(
                contents: contents,
                start: skip,
                size: contents.Count,
                totalSize: total
            );

            return result;
        }
    }
}