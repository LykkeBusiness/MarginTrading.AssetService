using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfileWithTemplate : ClientProfile
    {
        public string ClientProfileTemplateId { get; }

        public ClientProfileWithTemplate(string id,
            string regulatoryProfileId,
            string templateId,
            bool isDefault = false) : base(id, regulatoryProfileId, isDefault)
        {
            ClientProfileTemplateId = templateId;
        }
    }
}