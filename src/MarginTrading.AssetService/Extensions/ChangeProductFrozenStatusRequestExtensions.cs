using MarginTrading.AssetService.Contracts.Products;

namespace MarginTrading.AssetService.Extensions
{
    public static class ChangeProductFrozenStatusRequestExtensions
    {
        public static bool IsForceFreeze(this ChangeProductFrozenStatusRequest request)
        {
            return request.FreezeInfo?.Reason == ProductFreezeReasonContract.Manual;
        }
    }
}