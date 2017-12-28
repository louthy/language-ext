using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseDoubleTests : AbstractParseTTests<double>
    {
        protected override Option<double> ParseT(string value) => Prelude.parseDouble(value);

        [Theory]
        [InlineData(0.5)]
        [InlineData(-0.5)]
        [InlineData(double.Epsilon)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void parseDouble_ValidStringFromGiven_SomeAsGiven(double value) =>
            ParseT_ValidStringFromGiven_SomeAsGiven(value);
    }
}
