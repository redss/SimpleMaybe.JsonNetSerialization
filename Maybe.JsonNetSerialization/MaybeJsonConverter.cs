using System;
using Newtonsoft.Json;
using SimpleMaybe;

namespace Maybe.JsonNetSerialization
{
    public class MaybeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
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
