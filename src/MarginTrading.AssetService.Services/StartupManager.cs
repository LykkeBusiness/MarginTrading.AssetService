// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly ICqrsEngine _cqrsEngine;

        public StartupManager(ILog log, ICqrsEngine cqrsEngine)
        {
            _log = log;
            _cqrsEngine = cqrsEngine;
        }

        public async Task StartAsync()
        {
            _cqrsEngine.StartSubscribers();
            _cqrsEngine.StartProcesses();

            await Task.CompletedTask;
        }
    }
}