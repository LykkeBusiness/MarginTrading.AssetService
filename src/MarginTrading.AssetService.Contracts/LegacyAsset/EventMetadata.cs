// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    public class EventMetadata
    {
        public EventMetadata(string eventId, DateTime eventCreationDate)
        {
            EventId = eventId;
            EventCreationDate = eventCreationDate;
        }

        public string EventId { get; set; }

        public DateTime EventCreationDate { get; set; }
    }
}
