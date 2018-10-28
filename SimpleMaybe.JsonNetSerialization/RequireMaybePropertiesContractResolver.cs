using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleMaybe.JsonNetSerialization
{
    /// <summary>
    /// This contract resolver will require all serialized/deserialized objects'
    /// properties to be present and not null, except Maybe&lt;T&gt; objects.
    ///
    /// In other words - if you want to receive or send nulls, use Maybe&lt;T&gt;!
    /// 
    /// It's a IContractResolver decorator, so it can be used with any other
    /// contract resolver, e. g. DefaultContractResolver.
    /// </summary>
    public class RequireMaybePropertiesContractResolver : IContractResolver
    {
        private readonly IContractResolver _innerResolver;

        public RequireMaybePropertiesContractResolver(IContractResolver innerResolver)
        {
            _innerResolver = innerResolver;
        }

        public JsonContract ResolveContract(Type type)
        {
            var resolveContract = _innerResolver.ResolveContract(type);

            if (resolveContract is JsonObjectContract objectContract)
            {
                foreach (var jsonProperty in objectContract.Properties)
                {
                    if (!IsMaybe(jsonProperty.PropertyType))
                    {
                        jsonProperty.Required = Required.Always;
                    }
                }
            }

            return resolveContract;
        }

        private static bool IsMaybe(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Maybe<>);
        }
    }
}