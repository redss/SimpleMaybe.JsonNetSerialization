using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

// ReSharper disable ClassNeverInstantiated.Local

namespace SimpleMaybe.JsonNetSerialization.Tests
{
    public class RequireMaybePropertiesContractResolverTests
    {
        [Test]
        public void nulls_are_not_allowed_when_deserializing_into_something_else_than_maybe()
        {
            Deserializing<ContractWithReference>(@"{ ""value"": null }").Should().Throw<JsonSerializationException>();
            Deserializing<ContractWithValue>(@"{ ""value"": null }").Should().Throw<JsonSerializationException>();
            Deserializing<ContractWithNullable>(@"{ ""value"": null }").Should().Throw<JsonSerializationException>();
        }
        
        [Test]
        public void nulls_are_allowed_when_deserializing_into_maybe()
        {
            Deserialize<ContractWithReferenceMaybe>(@"{ ""value"": null }").Value.Should().Be(Maybe.None<string>());
            Deserialize<ContractWithValueMaybe>(@"{ ""value"": null }").Value.Should().Be(Maybe.None<int>());
        }

        [Test]
        public void deserializing_non_null_values_should_work_as_usual()
        {
            Deserialize<ContractWithReference>(@"{ ""value"": ""some value"" }").Value.Should().Be("some value");
            Deserialize<ContractWithValue>(@"{ ""value"": 123 }").Value.Should().Be(123);
            Deserialize<ContractWithNullable>(@"{ ""value"": 123 }").Value.Should().Be(123);

            Deserialize<ContractWithReferenceMaybe>(@"{ ""value"": ""some value"" }").Value.Should().Be(Maybe.Some("some value"));
            Deserialize<ContractWithValueMaybe>(@"{ ""value"": 123 }").Value.Should().Be(Maybe.Some(123));
        }

        [Test]
        public void serializing_null_is_not_allowed()
        {
            Serializing(new ContractWithReference { Value = null }).Should().Throw<JsonSerializationException>();
            Serializing(new ContractWithNullable { Value = null }).Should().Throw<JsonSerializationException>();
        }

        [Test]
        public void serializing_none_values_into_nulls_is_allowed()
        {
            Serialize(new ContractWithReferenceMaybe { Value = Maybe.None<string>() }).Should().Contain(@"""value"":null");
            Serialize(new ContractWithValueMaybe { Value = Maybe.None<int>() }).Should().Contain(@"""value"":null");
        }

        [Test]
        public void serializing_non_null_values_should_work_as_usual()
        {
            Serialize(new ContractWithReference { Value = "some-value" }).Should().Contain("some-value");
            Serialize(new ContractWithValue { Value = 123 }).Should().Contain("123");
            Serialize(new ContractWithNullable { Value = 123 }).Should().Contain("123");

            Serialize(new ContractWithReferenceMaybe { Value = Maybe.Some("some-value") }).Should().Contain("some-value");
            Serialize(new ContractWithValueMaybe { Value = Maybe.Some(123) }).Should().Contain("123");
        }

        class ContractWithReference
        {
            public string Value { get; set; }
        }

        class ContractWithValue
        {
            public int Value { get; set; }
        }

        class ContractWithNullable
        {
            public int? Value { get; set; }
        }

        class ContractWithReferenceMaybe
        {
            public Maybe<string> Value { get; set; }
        }

        class ContractWithValueMaybe
        {
            public Maybe<int> Value { get; set; }
        }

        private static Action Deserializing<T>(string json)
        {
            return () => Deserialize<T>(json);
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings());
        }

        private static Action Serializing(object value)
        {
            return () => Serialize(value);
        }

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings());
        }

        private static JsonSerializerSettings JsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new RequireMaybePropertiesContractResolver(
                    new CamelCasePropertyNamesContractResolver()
                )
            };

            settings.Converters.Add(new MaybeJsonConverter());

            return settings;
        }
    }
}