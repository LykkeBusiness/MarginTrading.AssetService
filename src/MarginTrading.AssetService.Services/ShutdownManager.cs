// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Common;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    // NOTE: Sometimes, shutdown process should be expressed explicitly. 
    // If this is your case, use this class to manage shutdown.
    // For example, sometimes some state should be saved only after all incoming message processing and 
    // all periodical handler was stopped, and so on.

    public class ShutdownManager : IShutdownManager
    {
        private readonly ILogger<ShutdownManager> _logger;
        private readonly IEnumerable<IStopable> _items;
        private readonly IEnumerable<IStartStop> _stopables;

        public ShutdownManager(IEnumerable<IStopable> items, IEnumerable<IStartStop> stopables, ILogger<ShutdownManager> logger)
        {
            _items = items;
            _stopables = stopables;
            _logger = logger;
        }

        public void Stop()
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
                    _logger.LogWarning(ex, "Unable to stop {ComponentName}", item.GetType().Name);
                }
            }
        }
    }
}
