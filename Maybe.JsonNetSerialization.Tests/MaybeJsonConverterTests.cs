using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleMaybe;

namespace Maybe.JsonNetSerialization.Tests
{
    public class MaybeJsonConverterTests
    {
        [Test]
        public void can_deserialize_null()
        {
            var someContract = Deserialize<SomeContract>(@"{ ""MaybeInt"": null }");

            someContract.MaybeInt.Should().Be(SimpleMaybe.Maybe.None<int>());
        }

        [Test]
        public void can_deserialize_undefined()
        {
            var someContract = Deserialize<SomeContract>(@"{ }");

            someContract.MaybeInt.Should().Be(SimpleMaybe.Maybe.None<int>());
        }

        [Test]
        public void can_deserialize_some_value()
        {
            var someContract = Deserialize<SomeContract>(@"{ ""MaybeInt"": 10 }");

            someContract.MaybeInt.Should().Be(SimpleMaybe.Maybe.Some(10));
        }

        [Test]
        public void can_deserialize_complex_objects()
        {
            var someContract = Deserialize<SomeComplexContract>(@"{ ""MaybeAddress"": { ""IsNorthPole"": true, ""Country"": ""Sealand"" } }");

            someContract.MaybeAddress.ValueOrDefault().Should().BeEquivalentTo(new SomeComplexContract.Address
            {
                IsNorthPole = true,
                Country = "Sealand"
            });
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new MaybeJsonConverter());
        }

        class SomeContract
        {
            public Maybe<int> MaybeInt { get; set; }
        }

        class SomeComplexContract
        {
            public Maybe<Address> MaybeAddress { get; set; }

            public class Address
            {
                public bool IsNorthPole { get; set; }
                public string Country { get; set; }
            }
        }
    }
}