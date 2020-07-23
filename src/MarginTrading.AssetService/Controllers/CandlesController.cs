// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Candles;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Settings.Candles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    [Authorize]
    [Route("api/candles")]
    [ApiController]
    public class CandlesController : ControllerBase, ICandlesSettingsApi
    {
        private readonly CandlesShardingSettings _candlesShardingSettings;

        private static readonly CandlesConsumerSettingsContract DefaultShardSettings = new CandlesConsumerSettingsContract
            {Name = DefaultShardName};
        
        private const string DefaultShardName = "default";

        public CandlesController(CandlesShardingSettings candlesShardingSettings)
        {
            _candlesShardingSettings = candlesShardingSettings;
        }

        [HttpGet]
        [Route("producer")]
        public Task<CandlesProducerSettingsContract> GetProducerSettingsAsync()
        {
            return Task.FromResult(new CandlesProducerSettingsContract
            {
                DefaultShardName = _candlesShardingSettings?.DefaultShardName ?? DefaultShardName,
                Shards = _candlesShardingSettings?.Shards?
                    .Select(x => x.ToContract()) ?? Enumerable.Empty<CandlesShardSettingsContract>()
            });
        }

        [HttpGet]
        [Route("consumer")]
        public Task<CandlesConsumerSettingsContract> GetConsumerSettingsAsync([FromQuery] string shardName)
        {
            if (string.IsNullOrWhiteSpace(shardName) || shardName == DefaultShardName)
            {
                return Task.FromResult(DefaultShardSettings);
            }

            var shardSettings = _candlesShardingSettings?.Shards?.FirstOrDefault(x => x.Name == shardName);

            return Task.FromResult(shardSettings?.ToConsumerContract());
        }
    }
}