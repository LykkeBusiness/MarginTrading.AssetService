using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MarginTrading.SettingsService.Contracts.Common
{
    /// <summary>
    /// Paginated response wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResponseContract<T>
    {
        /// <summary>
        /// Paginated sorted contents
        /// </summary>
        [NotNull]
        public IReadOnlyList<T> Contents { get; }
        
        /// <summary>
        /// Start position in total contents
        /// </summary>
        public int Start { get; }
        
        /// <summary>
        /// Size of returned contents
        /// </summary>
        public int Size { get; }
        
        /// <summary>
        /// Total size of all the contents
        /// </summary>
        public int TotalSize { get; }

        public PaginatedResponseContract([NotNull] IReadOnlyList<T> contents, int start, int size, int totalSize)
        {
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            Start = start;
            Size = size;
            TotalSize = totalSize;
        }
    }
}