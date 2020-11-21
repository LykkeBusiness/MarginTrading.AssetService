using System;
using System.Linq;
using System.Threading.Tasks;
using BookKeeper.Client.Workflow.Events;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class StartProductsSaga
    {
        private readonly IProductsRepository _productsRepository;
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly ILog _log;

        public StartProductsSaga(IProductsRepository productsRepository,
            CqrsContextNamesSettings _contextNames,
            ILog log)
        {
            _productsRepository = productsRepository;
            this._contextNames = _contextNames;
            _log = log;
        }

        [UsedImplicitly]
        public async Task Handle(EodProcessFinishedEvent e, ICommandSender sender)
        {
            var productsResult = await _productsRepository.GetAllAsync(null, null, isStarted: false);

            if (productsResult.IsSuccess && productsResult.Value != null && productsResult.Value.Any())
            {
                var endOfNextDay = e.TradingDay.Date.AddDays(2);
                var productsToStart = productsResult.Value
                    .Where(x => x.StartDate < endOfNextDay)
                    .ToList();
                
                _log.WriteInfo(nameof(StartProductsSaga), nameof(Handle), 
                    $"Found {productsToStart.Count} products that need to be started. Ids are: {string.Concat(',', productsToStart.Select(x => x.ProductId))}" 
                    );

                foreach (var product in productsToStart)
                {
                    sender.SendCommand(new StartProductCommand()
                    {
                        ProductId = product.ProductId,
                        OperationId = e.OperationId,
                    }, _contextNames.AssetService);
                }
            }
        }
    }
}