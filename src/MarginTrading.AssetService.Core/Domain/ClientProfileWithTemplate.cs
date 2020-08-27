using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfileWithTemplate : ClientProfile
    {
        public Guid? ClientProfileTemplateId { get; set; }
    }
}