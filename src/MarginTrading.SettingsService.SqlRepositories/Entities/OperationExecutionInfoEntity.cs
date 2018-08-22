using System;
using MarginTrading.SettingsService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.SettingsService.SqlRepositories.Entities
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