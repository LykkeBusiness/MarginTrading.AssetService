using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IProductsDiscontinueService
    {
        Task<(Product OldValue, Product NewValue, bool IsSuccess)> ChangeSuspendStatusAsync(string productId,
            bool isSuspended,
            string username, string correlationId);
    }
}