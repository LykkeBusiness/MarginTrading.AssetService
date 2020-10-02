using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.ClientProfiles
{
    public class ClientProfileChangedProjection
    {
        private readonly IReferentialDataChangedHandler _referentialDataChangedHandler;
        private readonly IConvertService _convertService;

        public ClientProfileChangedProjection(IReferentialDataChangedHandler referentialDataChangedHandler, IConvertService convertService)
        {
            _referentialDataChangedHandler = referentialDataChangedHandler;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(ClientProfileChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                case ChangeType.Edition:

                    var old = e.OldValue != null
                        ? _convertService.Convert<ClientProfileContract, ClientProfile>(e.OldValue)
                        : null;

                    var updated = _convertService.Convert<ClientProfileContract, ClientProfile>(e.NewValue);

                    await _referentialDataChangedHandler.HandleClientProfileUpserted(old, updated);

                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}