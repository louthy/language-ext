using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Parsing
{
    public abstract class AbstractParseTTests<T> where T : struct
    {
        protected abstract Option<T> ParseT(string value);

        [Fact]
        public void ParseT_NullString_None()
        {
            Option<T> expected = None;
            var actual = ParseT(null);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseT_EmptyString_None()
        {
            Option<T> expected = None;
            var actual = ParseT("");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseT_ValidStringFromDefaultValue_SomeDefaultValue() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(default(T));

        protected void ParseT_ValidStringFromGiven_SomeAsGiven(T expected)
        {
            var value = expected.ToString();
            var actual = ParseT(value);
            Assert.Equal(expected, actual);
        }

    }
}
