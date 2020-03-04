// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class FreezeInfo
    {
        public FreezeReason Reason { get; set; }
        
        public string Comment { get; set; }
        
        public DateTime? UnfreezeDate { get; set; }
    }
}