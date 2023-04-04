using System;

using Newtonsoft.Json;

namespace MarginTrading.AssetService.Core.Domain
{
    /// <summary>
    /// Value object representing a product contract size.
    /// </summary>
    public readonly struct ContractSize : IEquatable<ContractSize>, IComparable<ContractSize>
    {
        private readonly uint _value;

        /// <summary>
        /// Creates a new instance of <see cref="ContractSize"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException">When value is less than 1.</exception>
        public ContractSize(int value)
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Value {value} is invalid. Contract size must be greater than or equal to 1.");
            }

            _value = (uint)value;
        }
    
        public int Value
        {
            get { return (int)_value; }
        }
    
        public static ContractSize Default
        {
            get { return new ContractSize(1); }
        }

        public static implicit operator uint(ContractSize size)
        {
            return size._value;
        }

        public static implicit operator int(ContractSize size)
        {
            return (int)size._value;
        }
        
        public static implicit operator ContractSize(uint value)
        {
            return new ContractSize((int)value);
        }
        
        public static implicit operator ContractSize(int value)
        {
            return new ContractSize(value);
        }
    
        public static bool operator ==(ContractSize left, ContractSize right)
        {
            return left._value == right._value;
        }
    
        public static bool operator !=(ContractSize left, ContractSize right)
        {
            return !(left == right);
        }
    
        public static bool operator <(ContractSize left, ContractSize right)
        {
            return left._value < right._value;
        }
    
        public static bool operator <=(ContractSize left, ContractSize right)
        {
            return left._value <= right._value;
        }
    
        public static bool operator >(ContractSize left, ContractSize right)
        {
            return left._value > right._value;
        }
    
        public static bool operator >=(ContractSize left, ContractSize right)
        {
            return left._value >= right._value;
        }
    
        public override bool Equals(object obj)
        {
            if (!(obj is ContractSize))
            {
                return false;
            }
    
            return Equals((ContractSize)obj);
        }
    
        public bool Equals(ContractSize other)
        {
            return _value == other._value;
        }
    
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public int CompareTo(ContractSize other)
        {
            return _value.CompareTo(other._value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    /// <summary>
    /// Converts <see cref="ContractSize"/> to and from JSON using its <see cref="ContractSize.Value"/> property.
    /// Converter is used by Newtonsoft.Json.JsonSerializer.
    /// </summary>
    public class ContractSizeConverter : JsonConverter<ContractSize>
    {
        public override void WriteJson(JsonWriter writer, ContractSize value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value);
        }

        public override ContractSize ReadJson(JsonReader reader,
            Type objectType,
            ContractSize existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var source = reader.Value;
            if (source == null)
            {
                return ContractSize.Default;
            }

            try
            {
                var parsedInt = Convert.ToInt32(source);
                return new ContractSize(parsedInt);
            }
            catch (Exception e)
            {
                if (e is FormatException || e is OverflowException || e is InvalidCastException)
                {
                    return ContractSize.Default;
                }

                if (e is ArgumentOutOfRangeException)
                {
                    return ContractSize.Default;
                }

                throw;
            }
        }
    }
}