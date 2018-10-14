using System;
using System.Reflection;

// ReSharper disable PossibleNullReferenceException

namespace SimpleMaybe.JsonNetSerialization
{
    internal static class MaybeNonGenericFactory
    {
        public static object None(Type valueType)
        {
            return typeof(MaybeNonGenericFactory)
                .GetMethod(nameof(MaybeNone), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                .Invoke(null, new object[0]);
        }

        private static Maybe<TValue> MaybeNone<TValue>()
        {
            return Maybe.None<TValue>();
        }

        public static object TryGetValue(object maybe, Type valueType)
        {
            return typeof(MaybeNonGenericFactory)
                .GetMethod(nameof(MaybeTryGetValue), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                .Invoke(null, new[] { maybe });
        }

        private static object MaybeTryGetValue<TValue>(Maybe<TValue> maybe)
        {
            return maybe.Match<object>(
                some: value => value,
                none: () => null
            );
        }

        public static object Some(Type valueType, object value)
        {
            return typeof(MaybeNonGenericFactory)
                .GetMethod(nameof(MaybeSome), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType)
                .Invoke(null, new[] { value });
        }

        private static Maybe<TValue> MaybeSome<TValue>(TValue value)
        {
            return Maybe.Some(value);
        }
    }
}