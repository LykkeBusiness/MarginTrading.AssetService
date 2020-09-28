using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.Underlyings
{
    public class UnderlyingsProjection
    {
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly IConvertService _convertService;

        public UnderlyingsProjection(IUnderlyingsCache underlyingsCache, IConvertService convertService)
        {
            _underlyingsCache = underlyingsCache;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(UnderlyingChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    _underlyingsCache.AddOrUpdateByMdsCode(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue));
                    break;
                case ChangeType.Edition:
                    //If mds code was updated delete the existing record in the cache first
                    if(e.OldValue.MdsCode != e.NewValue.MdsCode)
                        _underlyingsCache.Remove(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.OldValue));

                    _underlyingsCache.AddOrUpdateByMdsCode(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue));
                    break;
                case ChangeType.Deletion:
                    _underlyingsCache.Remove(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.OldValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}