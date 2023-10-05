// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MarginTrading.AssetService.Contracts
{
    public abstract class BaseCache<T> : IBaseCache<T>
    {
        protected ConcurrentDictionary<string, T> _cache = new ConcurrentDictionary<string, T>();
        protected readonly SemaphoreSlim _initializeLock = new SemaphoreSlim(1, 1);
        
        public abstract Task StartAsync();

        public virtual bool TryGetValue(string key, out T result) => _cache.TryGetValue(key, out result);

        public virtual void AddOrUpdate(T item)
        {
            _cache[GetKey(item)] = item;
        }

        public virtual bool Remove(T item) => _cache.TryRemove(GetKey(item), out _);

        protected abstract string GetKey(T item);
    }
}