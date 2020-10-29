using System;
using System.Linq;
using System.Threading.Tasks;
using BookKeeper.Client.Workflow.Events;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class EodProcessFinishedProjection
    {
        private readonly IProductsService _productsService;
        private readonly ILog _log;

        public EodProcessFinishedProjection(
            // IProductsService productsService, 
            ILog log)
        {
            // _productsService = productsService;
            _log = log;
        }

        [UsedImplicitly]
        public async Task Handle(EodProcessFinishedEvent e)
        {
            return;
            var productsResult = await _productsService.GetAllAsync(null, null, isStarted: false);

            if (productsResult.IsSuccess && productsResult.Value != null && productsResult.Value.Any())
            {
                var endOfNextDay = DateTime.UtcNow.Date.AddDays(2);
                var productsToStart = productsResult.Value
                    .Where(x => x.StartDate < endOfNextDay)
                    .ToList();


                foreach (var product in productsToStart)
                {
                    product.IsStarted = true;
                }

                _log.WriteInfo(nameof(EodProcessFinishedProjection), nameof(Handle),
                    $"Attempting to start products: {string.Join(',', productsToStart.Select(x => x.ProductId))}");

                var result = await _productsService.UpdateBatchAsync(productsToStart, "system", e.OperationId);
                // todo: batch update stops on first error. Should we try to continue anyway?
                if (result.IsFailed)
                {
                    var exception =
                        new Exception($"Some of the products have not been started: {result.Error.ToString()}");
                    _log.WriteError(nameof(EodProcessFinishedProjection), nameof(Handle), exception);
                    throw exception;
                }
            }
        }
    }
}