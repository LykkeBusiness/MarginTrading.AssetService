using System;

using FsCheck;
using FsCheck.Xunit;

using MarginTrading.AssetService.Core.Domain;

using Newtonsoft.Json;

using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public sealed class ContractSizeTests
    {
        class StubForSerialization
        {
            public ContractSize ContractSize { get; set; }
        }

        static class Gens
        {
            internal static Gen<ulong> GreaterThenInteger()
            {
                var addition = Gen.Choose(1, Int32.MaxValue);
                return addition.Select(a => (ulong)(a + Int32.MaxValue));
            }
        }

        [Property]
        public void ContractSize_MustBeGreaterThanZero(NonNegativeInt value)
        {
            var invalidValue = -value.Item;
            Assert.Throws<ArgumentOutOfRangeException>(() => new ContractSize(invalidValue));
        }

        [Property]
        public Property ContractSize_Equality(PositiveInt value1, PositiveInt value2)
        {
            var size1 = new ContractSize(value1.Item);
            var size2 = new ContractSize(value2.Item);

            var equality = Prop.When(value1.Item == value2.Item, () => size1 == size2);
            var inequality = Prop.When(value1.Item != value2.Item, () => size1 != size2);

            return equality.Or(inequality);
        }

        [Property]
        public Property ContractSize_ComparisonLess(PositiveInt value1, PositiveInt value2)
        {
            var size1 = new ContractSize(value1.Item);
            var size2 = new ContractSize(value2.Item);

            var lessThan = Prop.When(value1.Item < value2.Item, () => size1 < size2);
            var lessThanOrEqual = Prop.When(value1.Item <= value2.Item, () => size1 <= size2);

            return lessThan.And(lessThanOrEqual);
        }

        [Property]
        public Property ContractSize_ComparisonGreater(PositiveInt value1, PositiveInt value2)
        {
            var size1 = new ContractSize(value1.Item);
            var size2 = new ContractSize(value2.Item);

            var greaterThan = Prop.When(value1.Item > value2.Item, () => size1 > size2);
            var greaterThanOrEqual = Prop.When(value1.Item >= value2.Item, () => size1 >= size2);

            return greaterThan.And(greaterThanOrEqual);
        }
        
        [Property]
        public void ContractSize_ToString_IsEqualToValue(PositiveInt value)
        {
            var size = new ContractSize(value.Item);

            Assert.Equal(value.Item.ToString(), size.ToString());
        }

        [Property]
        public void ContractSize_Serialization_AsValueObject(PositiveInt value)
        {
            var original = new StubForSerialization { ContractSize = new ContractSize(value.Item) };

            var json = JsonConvert.SerializeObject(original, new ContractSizeConverter());

            Assert.Contains($@"""ContractSize"":{value.Item}", json);
        }

        [Property]
        public void ContractSize_Deseialization_AsValueObject(PositiveInt value)
        {
            var json = $@"{{""ContractSize"":{value.Item}}}";

            var deserialized = JsonConvert.DeserializeObject<StubForSerialization>(json, new ContractSizeConverter());

            Assert.Equal(new ContractSize(value.Item), deserialized.ContractSize);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("not a number")]
        public void ContractSize_Deseialization_NotANumber_Returns_DefaultValue(string notANumber)
        {
            var json = $@"{{""ContractSize"":""{notANumber}""}}";

            var deserialized = JsonConvert.DeserializeObject<StubForSerialization>(json, new ContractSizeConverter());

            Assert.Equal(ContractSize.Default, deserialized.ContractSize);
        }

        [Property]
        public Property ContractSize_Deserialization_Overflow_Returns_DefaultValue()
        {
            return Prop.ForAll((from overflow in Gens.GreaterThenInteger() select overflow).ToArbitrary(), o =>
            {
                var json = $@"{{""ContractSize"":{o}}}";
                var deserialized =
                    JsonConvert.DeserializeObject<StubForSerialization>(json, new ContractSizeConverter());
                Assert.Equal(ContractSize.Default, deserialized.ContractSize);
            });
        }

        [Property]
        public void ContractSize_Deserialization_OutOfRange_Returns_DefaultValue(PositiveInt value)
        {
            var json = $@"{{""ContractSize"":""{-value.Item}""}}";

            var deserialized = JsonConvert.DeserializeObject<StubForSerialization>(json, new ContractSizeConverter());

            Assert.Equal(ContractSize.Default, deserialized.ContractSize);
        }
    }
}