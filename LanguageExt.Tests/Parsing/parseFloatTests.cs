using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseFloatTests : AbstractParseTTests<float>
    {
        protected override Option<float> ParseT(string value) => Prelude.parseFloat(value);

        [Theory]
        [InlineData(0.5)]
        [InlineData(-0.5)]
        [InlineData(float.Epsilon)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        //[InlineData(float.NaN)] TODO -- Why is this here?
        public void parseFloat_ValidStringFromGiven_SomeAsGiven(float value) =>
            ParseT_ValidStringFromGiven_SomeAsGiven(value);
    }
}
