using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IAuditRepository
    {
        Task InsertAsync(IAuditModel model);

        Task<IReadOnlyList<IAuditModel>> GetAll(int? year, int? month);
    }
}