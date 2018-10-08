using System;

namespace Maybe.JsonNetSerialization
{
    internal static class MaybeNonGenericFactory
    {
        public static object None(Type type)
        {
            var noneMethod = typeof(SimpleMaybe.Maybe).GetMethod(nameof(SimpleMaybe.Maybe.None));

            var genericNoneMethod = noneMethod.MakeGenericMethod(type);

            return genericNoneMethod.Invoke(null, new object[0]);
        }

        public static object Some(Type type, object value)
        {
            var noneMethod = typeof(SimpleMaybe.Maybe).GetMethod(nameof(SimpleMaybe.Maybe.Some));

            var genericNoneMethod = noneMethod.MakeGenericMethod(type);

            return genericNoneMethod.Invoke(null, new[] { value });
        }
    }
}