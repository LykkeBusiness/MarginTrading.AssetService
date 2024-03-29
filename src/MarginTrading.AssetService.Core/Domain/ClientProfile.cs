﻿using System;
using Lykke.Snow.Audit.Abstractions;
using MarginTrading.AssetService.Core.Exceptions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfile : IAuditableObject<AuditDataType>
    {
        private const string ClientProfileDefaultId = "Default";
        public string Id { get; }
        public string RegulatoryProfileId { get; }
        public bool IsDefault { get; }

        public ClientProfile(string id, string regulatoryProfileId, bool isDefault = false)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrEmpty(regulatoryProfileId))
                throw new ArgumentNullException(nameof(regulatoryProfileId));

            if (isDefault && id != ClientProfileDefaultId)
                throw new ClientProfileNonDefaultUpdateForbiddenException();
            
            (Id, RegulatoryProfileId, IsDefault) = (id, regulatoryProfileId, isDefault);
        }

        public ClientProfile ChangeDefault(bool isDefault) => new ClientProfile(Id, RegulatoryProfileId, isDefault);
        public AuditDataType GetAuditDataType() => AuditDataType.ClientProfile;
        public string GetAuditReference() => Id;
    }
}