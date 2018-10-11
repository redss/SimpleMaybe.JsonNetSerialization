using FluentAssertions;
using NUnit.Framework;

namespace SimpleMaybe.JsonNetSerialization.Tests
{
    public class MaybeNonGenericCreationTests
    {
        [Test]
        public void can_create_none()
        {
            var none = MaybeNonGenericFactory.None(typeof(int));

            none.Should().Be(SimpleMaybe.Maybe.None<int>());
        }

        [Test]
        public void can_create_maybe()
        {
            var some = MaybeNonGenericFactory.Some(typeof(int), 10);

            some.Should().Be(SimpleMaybe.Maybe.Some(10));
        }

        [Test]
        public void can_extract_some_out_of_maybe()
        {
            var some = SimpleMaybe.Maybe.Some(10);

            var hasValue = MaybeNonGenericFactory.TryGetValue(some, typeof(int), out var value);

            hasValue.Should().Be(true);
            value.Should().Be(10);
        }
    }
}