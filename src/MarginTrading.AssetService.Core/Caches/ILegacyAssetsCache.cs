using System;
using System.Collections.Generic;
using Cronut.Dto.Assets;

namespace MarginTrading.AssetService.Core.Caches
{
    public interface ILegacyAssetsCache
    {
        void Start();

        Asset GetById(string id);

        List<Asset> GetByFilter(Func<Asset, bool> filter);

        List<Asset> GetAll();

        void AddOrUpdateMultiple(IEnumerable<Asset> assets);

        void Remove(Asset asset);
    }
}