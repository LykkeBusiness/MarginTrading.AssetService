using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class AssetTypeWithTemplate : AssetType
    {
        public Guid? AssetTypeTemplateId { get; set; }
    }
}