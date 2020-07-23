﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class OperationExecutionInfoEntity : IOperationExecutionInfo<object>
    {
        public string OperationName { get; set; }
        
        public string Id { get; set; }
        public DateTime LastModified { get; set; }

        object IOperationExecutionInfo<object>.Data => JsonConvert.DeserializeObject<object>(Data);
        public string Data { get; set; }
        
    }
}