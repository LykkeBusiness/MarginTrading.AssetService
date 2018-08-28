using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts.AssetPair;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface ICqrsMessageSender
    {
        Task SendAssetPairChangedEvent(AssetPairChangedEvent @event);
    }
}