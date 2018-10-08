using System;
using Newtonsoft.Json;
using SimpleMaybe;

namespace Maybe.JsonNetSerialization
{
    public class MaybeJsonConverter : JsonConverter
    {
        // todo: test
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType().GetGenericArguments()[0];

            if (MaybeNonGenericFactory.TryGetValue(value, valueType, out var innerValue))
            {
                serializer.Serialize(writer, innerValue);
            }
            else
            {
                serializer.Serialize(writer, null);
            }
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
