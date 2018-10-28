using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SimpleMaybe;
using SimpleMaybe.JsonNetSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// purpose of this file is to provide code for readme/documentation

// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ObjectCreationAsStatement

namespace SimpleMaybe.JsonNetSerialization.DocumentationExamples
{
    public class ReadmeExamples
    {
        class before_refactoring_to_maybe
        {
            public class BuyTicketRequest
            {
                public string EventId { get; set; }
                public string FullName { get; set; }
                public string DiscountCode { get; set; }
            }
        }

        public class BuyTicketRequest
        {
            public string EventId { get; set; }
            public Maybe<string> FullName { get; set; }
            public Maybe<string> DiscountCode { get; set; }
        }

        [Test]
        public void maybe_deserialization_example()
        {
            var json = File.ReadAllText("BuyTicketRequest.json");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new RequireMaybePropertiesContractResolver(
                    new DefaultContractResolver()
                ),
                Converters = new JsonConverter[]
                {
                    new MaybeJsonConverter()
                }
            };

            var request = JsonConvert.DeserializeObject<BuyTicketRequest>(json, settings);

            request.EventId.Should().Be("modal-nodes-05-1977");
            request.FullName.Should().Be(Maybe.Some("Han Solo"));
            request.DiscountCode.Should().Be(Maybe.None<string>());
        }

        [Test]
        public void null_smuggling_example()
        {
            var json = File.ReadAllText("BuyTicketRequest-invalid.json");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new RequireMaybePropertiesContractResolver(
                    new DefaultContractResolver()
                ),
                Converters = new JsonConverter[]
                {
                    new MaybeJsonConverter()
                }
            };

            //JsonConvert.DeserializeObject<BuyTicketRequest>(json, settings);

            Action deserializing = () =>
            {
                JsonConvert.DeserializeObject<BuyTicketRequest>(json, settings);
            };

            deserializing.Should().Throw<JsonSerializationException>();
        }

        [Test]
        public void use_contract_resolver()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new RequireMaybePropertiesContractResolver(
                    new DefaultContractResolver()
                )
            };
        }

        [Test]
        public void contract_resolver_composition()
        {
            new RequireMaybePropertiesContractResolver(
                new DefaultContractResolver()
            );

            new RequireMaybePropertiesContractResolver(
                new CamelCasePropertyNamesContractResolver()
            );

            new RequireMaybePropertiesContractResolver(
                new YourFancyContractResolver()
            );
        }

        public class YourFancyContractResolver : IContractResolver
        {
            public JsonContract ResolveContract(Type type)
            {
                throw new NotImplementedException();
            }
        }
    }
}