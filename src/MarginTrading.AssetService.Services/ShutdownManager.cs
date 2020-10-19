// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services
{
    // NOTE: Sometimes, shutdown process should be expressed explicitly. 
    // If this is your case, use this class to manage shutdown.
    // For example, sometimes some state should be saved only after all incoming message processing and 
    // all periodical handler was stopped, and so on.

    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IEnumerable<IStopable> _items;
        private readonly IEnumerable<IStartStop> _stopables;

        public ShutdownManager(ILog log, IEnumerable<IStopable> items, IEnumerable<IStartStop> stopables)
        {
            _log = log;
            _items = items;
            _stopables = stopables;
        }

        public async Task StopAsync()
        {
            var allItems = _items.Concat(_stopables).Distinct();
            foreach (var item in allItems)
            {
                try
                {
                    item.Stop();
                }
                catch (Exception ex)
                {
                    _log.WriteWarning(nameof(StopAsync), null, $"Unable to stop {item.GetType().Name}", ex);
                }
            }

            await Task.CompletedTask;
        }
    }
}
