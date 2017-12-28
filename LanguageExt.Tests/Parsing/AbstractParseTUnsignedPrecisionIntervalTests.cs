using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public abstract class AbstractParseTUnsignedPrecisionIntervalTests<T>
        : AbstractParseTPrecisionIntervalTests<T>
        where T : struct
    {
        protected abstract T PositiveOne { get; }
        protected abstract T PositiveTwo { get; }

        [Fact]
        public void ParseT_ValidStringFromPositiveOne_SomePositiveOne() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(PositiveOne);

        [Fact]
        public void ParseT_ValidStringFromPositiveTwo_SomePositiveTwo() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(PositiveTwo);
    }
}
