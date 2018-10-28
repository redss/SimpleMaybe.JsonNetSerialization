using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

// ReSharper disable UnusedAutoPropertyAccessor.Local - they are by reflection
// ReSharper disable ClassNeverInstantiated.Local - they are by reflection

namespace SimpleMaybe.JsonNetSerialization.Tests
{
    public class MaybeJsonConverterTests
    {
        [Test]
        public void can_deserialize_none()
        {
            var someContract = Deserialize<SomeContract>(@"{ ""MaybeInt"": null }");

            someContract.MaybeInt.Should().Be(Maybe.None<int>());
        }

        [Test]
        public void can_serialize_none()
        {
            var json = Serialize(new SomeContract
            {
                MaybeInt = Maybe.None<int>()
            });

            JsonsShouldBeEqual(json, @"{ ""MaybeInt"": null }");
        }

        [Test]
        public void can_deserialize_undefined()
        {
            var someContract = Deserialize<SomeContract>(@"{ }");

            someContract.MaybeInt.Should().Be(Maybe.None<int>());
        }

        [Test]
        public void can_deserialize_some_value()
        {
            var someContract = Deserialize<SomeContract>(@"{ ""MaybeInt"": 10 }");

            someContract.MaybeInt.Should().Be(Maybe.Some(10));
        }

        [Test]
        public void can_serialize_some_value()
        {
            var json = Serialize(new SomeContract
            {
                MaybeInt = Maybe.Some(10)
            });

            JsonsShouldBeEqual(json, @"{ ""MaybeInt"": 10 }");
        }

        [Test]
        public void can_deserialize_complex_objects()
        {
            var someContract = Deserialize<SomeComplexContract>(@"{ ""MaybeAddress"": { ""IsAntarctica"": true, ""City"": ""McMurdo"" } }");

            someContract.MaybeAddress.ValueOrDefault().Should().BeEquivalentTo(new SomeComplexContract.Address
            {
                IsAntarctica = true,
                City = "McMurdo"
            });
        }

        [Test]
        public void can_serialize_complex_objects()
        {
            var json = Serialize(new SomeComplexContract
            {
                MaybeAddress = Maybe.Some(new SomeComplexContract.Address
                {
                    IsAntarctica = true,
                    City = "McMurdo"
                })
            });

            JsonsShouldBeEqual(json, @"{ ""MaybeAddress"": { ""IsAntarctica"": true, ""City"": ""McMurdo"" } }");
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new MaybeJsonConverter());
        }

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, new MaybeJsonConverter());
        }

        private static void JsonsShouldBeEqual(string actualJson, string expectedJson)
        {
            JToken.Parse(actualJson).ToString().Should().Be(JToken.Parse(expectedJson).ToString());
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
                public bool IsAntarctica { get; set; }
                public string City { get; set; }
            }
        }
    }
}