using FluentAssertions;
using NUnit.Framework;

namespace Maybe.JsonNetSerialization.Tests
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
    }
}