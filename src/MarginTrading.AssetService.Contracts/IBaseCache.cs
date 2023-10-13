// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Base cache interface
    /// </summary>
    public interface IBaseCache<T>
    {
        /// <summary>
        /// Initialize cache
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Get item by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns>
        /// True if item was found
        /// </returns>
        bool TryGetValue(string key, out T result);
        
        /// <summary>
        /// Adds or updates item in cache
        /// </summary>
        void AddOrUpdate(T item);
        
        /// <summary>
        /// Removes item from cache
        /// </summary>
        /// <returns>
        /// True if item was removed
        /// </returns>
        bool Remove(T item);
    }
}