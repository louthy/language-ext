using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseCharTests : AbstractParseTPrecisionIntervalTests<char>
    {
        protected override Option<char> ParseT(string value) => Prelude.parseChar(value);

        protected override char MinValue => char.MinValue;
        protected override char MaxValue => char.MaxValue;

        [Theory]
        [InlineData('a')]
        [InlineData('1')]
        [InlineData(' ')]
        public void parseChar_ValidStringFromGiven_SomeAsGiven(char value) =>
            ParseT_ValidStringFromGiven_SomeAsGiven(value);
    }
}
