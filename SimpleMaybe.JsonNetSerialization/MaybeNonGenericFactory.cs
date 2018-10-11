using System;

// ReSharper disable PossibleNullReferenceException

namespace SimpleMaybe.JsonNetSerialization
{
    internal static class MaybeNonGenericFactory
    {
        public static object None(Type type)
        {
            return typeof(Maybe)
                .GetMethod(nameof(Maybe.None))
                .MakeGenericMethod(type)
                .Invoke(null, new object[0]);
        }

        // todo: just return object or null?
        public static bool TryGetValue(object maybe, Type type, out object value)
        {
            var hasValue = (bool) typeof(Maybe<>)
                .MakeGenericType(type)
                .GetProperty(nameof(Maybe<object>.HasValue))
                .GetValue(maybe);

            if (hasValue)
            {
                value = typeof(Maybe<>)
                    .MakeGenericType(type)
                    .GetMethod(nameof(Maybe<object>.ValueOrFail))
                    .Invoke(maybe, new object[0]);

                return true;
            }

            value = null;

            return false;
        }

        public static object Some(Type type, object value)
        {
            return typeof(Maybe)
                .GetMethod(nameof(Maybe.Some))
                .MakeGenericMethod(type)
                .Invoke(null, new[] { value });
        }
    }
}