// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class InterestRate
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("mdsCode")]
        public string MdsCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public override bool Equals(object value)
        {
            return Equals(value as InterestRate);
        }

        public bool Equals(InterestRate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                String.Equals(Currency, other.Currency) &&
                String.Equals(MdsCode, other.MdsCode) &&
                String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, Currency) ? Currency.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, MdsCode) ? MdsCode.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(InterestRate obj, InterestRate other)
        {
            if (ReferenceEquals(obj, other)) return true;
            if (ReferenceEquals(null, obj)) return false;
            return (obj.Equals(other));
        }

        public static bool operator !=(InterestRate obj, InterestRate other)
        {
            return !(obj == other);
        }
    }
}