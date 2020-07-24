﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class OperationExecutionInfo<T> : IOperationExecutionInfo<T> 
        where T: class
    {
        public string OperationName { get; }
        public string Id { get; }
        public DateTime LastModified { get; }

        public T Data { get; }

        public OperationExecutionInfo([NotNull] string operationName, [NotNull] string id, DateTime lastModified, 
            [NotNull] T data)
        {
            OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            LastModified = lastModified;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}