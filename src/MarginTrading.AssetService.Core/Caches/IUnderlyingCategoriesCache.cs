using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.AssetService.Core.Caches
{
    public interface IUnderlyingCategoriesCache
    {
        Task<List<UnderlyingCategoryCacheModel>> Get();
    }
}