﻿namespace MarginTrading.AssetService.Core.Caches
{
    public interface IUnderlyingsCache
    {
        void Start();
        UnderlyingsCacheModel GetByMdsCode(string mdsCode);
        void AddOrUpdateByMdsCode(UnderlyingsCacheModel underlying);
        void AddOrUpdateByChangedMdsCode(string oldMdsCode, UnderlyingsCacheModel underlying);
        void Remove(UnderlyingsCacheModel underlying);
    }
}