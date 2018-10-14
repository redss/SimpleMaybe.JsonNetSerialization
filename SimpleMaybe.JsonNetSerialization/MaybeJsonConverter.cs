using System;
using Newtonsoft.Json;

namespace SimpleMaybe.JsonNetSerialization
{
    public class MaybeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object maybe, JsonSerializer serializer)
        {
            var valueType = maybe.GetType().GetGenericArguments()[0];

            var valueOrNull = MaybeNonGenericFactory.TryGetValue(maybe, valueType);

            serializer.Serialize(writer, valueOrNull);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var valueType = objectType.GetGenericArguments()[0];

            if (reader.TokenType == JsonToken.Undefined || reader.TokenType == JsonToken.Null)
            {
                return MaybeNonGenericFactory.None(valueType);
            }

            var deserialized = serializer.Deserialize(reader, valueType);

            return MaybeNonGenericFactory.Some(valueType, deserialized);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Maybe<>);
        }
    }
}
