using System;
using System.Linq;
using System.Threading.Tasks;
using BookKeeper.Client.Workflow.Events;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class StartProductsSaga
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMarketDayOffService _marketDayOffService;
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly ILogger<StartProductsSaga> _logger;

        public StartProductsSaga(IProductsRepository productsRepository,
            IMarketDayOffService marketDayOffService,
            CqrsContextNamesSettings contextNames,
            ILogger<StartProductsSaga> logger)
        {
            _productsRepository = productsRepository;
            _marketDayOffService = marketDayOffService;
            _contextNames = contextNames;
            _logger = logger;
        }

        [UsedImplicitly]
        public async Task Handle(EodProcessFinishedEvent e, ICommandSender sender)
        {
            var productsResult = await _productsRepository.GetAllAsync(null, null, null, isStarted: false);

            if (productsResult.IsSuccess && productsResult.Value != null && productsResult.Value.Any())
            {
                var products = productsResult.Value;
                var markets = products.Select(x => x.Market).Distinct().ToArray();
                var marketInfos = await _marketDayOffService.GetMarketsInfo(markets, null);

                var productsToStart = productsResult.Value
                    .Where(x =>
                    {
                        var marketNextTradingDayStart = marketInfos[x.Market].NextTradingDayStart;
                        return x.StartDate < DateOnly.FromDateTime(marketNextTradingDayStart.AddDays(1));
                    })
                    .ToList();

                var productsIdsString = string.Concat(',', productsToStart.Select(x => x.ProductId));
                _logger.LogInformation("Found {ProductsCount} products that need to be started. Ids are: {ProductsIdsString}", productsToStart.Count, productsIdsString);
                
                foreach (var product in productsToStart)
                {
                    sender.SendCommand(new StartProductCommand
                    {
                        ProductId = product.ProductId,
                        OperationId = e.OperationId
                    }, _contextNames.AssetService);
                }
            }
        }
    }
}