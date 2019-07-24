using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public abstract class AbstractParseTSignedPrecisionIntervalTests<T>
        : AbstractParseTPrecisionIntervalTests<T>
        where T : struct
    {
        protected abstract T NegativeOne { get; }
        protected abstract T PositiveOne { get; }

        [Fact]
        public void ParseT_ValidStringFromNegativeOne_SomeNegativeOne() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(NegativeOne);

        [Fact]
        public void ParseT_ValidStringFromPositiveOne_SomePositiveOne() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(PositiveOne);

    }
}
