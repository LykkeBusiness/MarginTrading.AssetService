using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Services
{
    public interface ICqrsEntityChangedSender
    {
        Task SendEntityCreatedEvent<TModel, TContract, TEvent>(TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;

        Task SendEntityEditedEvent<TModel, TContract, TEvent>(TModel oldValue,
            TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;

        Task SendEntityDeletedEvent<TModel, TContract, TEvent>(TModel oldValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;
    }
}