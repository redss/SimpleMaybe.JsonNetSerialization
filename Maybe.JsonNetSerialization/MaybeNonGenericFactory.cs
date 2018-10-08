using System;

// ReSharper disable PossibleNullReferenceException

namespace Maybe.JsonNetSerialization
{
    internal static class MaybeNonGenericFactory
    {
        public static object None(Type type)
        {
            return typeof(SimpleMaybe.Maybe)
                .GetMethod(nameof(SimpleMaybe.Maybe.None))
                .MakeGenericMethod(type)
                .Invoke(null, new object[0]);
        }

        public static bool TryGetValue(object maybe, Type type, out object value)
        {
            var hasValue = (bool) typeof(SimpleMaybe.Maybe<>)
                .MakeGenericType(type)
                .GetProperty(nameof(SimpleMaybe.Maybe<object>.HasValue))
                .GetValue(maybe);

            if (hasValue)
            {
                value = typeof(SimpleMaybe.Maybe<>)
                    .MakeGenericType(type)
                    .GetMethod(nameof(SimpleMaybe.Maybe<object>.ValueOrFail))
                    .Invoke(maybe, new object[0]);

                return true;
            }

            value = null;

            return false;
        }

        public static object Some(Type type, object value)
        {
            return typeof(SimpleMaybe.Maybe)
                .GetMethod(nameof(SimpleMaybe.Maybe.Some))
                .MakeGenericMethod(type)
                .Invoke(null, new[] { value });
        }
    }
}