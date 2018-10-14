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

            none.Should().Be(Maybe.None<int>());
        }

        [Test]
        public void can_create_maybe()
        {
            var some = MaybeNonGenericFactory.Some(typeof(int), 10);

            some.Should().Be(Maybe.Some(10));
        }

        [Test]
        public void can_extract_value_out_of_some()
        {
            MaybeNonGenericFactory.TryGetValue(Maybe.Some(10), typeof(int))
                .Should().Be(10);

            MaybeNonGenericFactory.TryGetValue(Maybe.Some<int?>(10), typeof(int?))
                .Should().Be(10);

            MaybeNonGenericFactory.TryGetValue(Maybe.Some("aaa"), typeof(string))
                .Should().Be("aaa");
        }

        [Test]
        public void can_extract_null_out_of_none()
        {
            MaybeNonGenericFactory.TryGetValue(Maybe.None<int>(), typeof(int))
                .Should().BeNull();

            MaybeNonGenericFactory.TryGetValue(Maybe.None<int?>(), typeof(int?))
                .Should().BeNull();

            MaybeNonGenericFactory.TryGetValue(Maybe.None<string>(), typeof(string))
                .Should().BeNull();
        }
    }
}