using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public abstract class AbstractParseTPrecisionIntervalTests<T>
        : AbstractParseTTests<T>
        where T : struct
    {
        protected abstract T MinValue { get; }
        protected abstract T MaxValue { get; }

        [Fact]
        public void ParseT_ValidStringFromMinValue_SomeMinValue() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(MinValue);

        [Fact]
        public void ParseT_ValidStringFromMaxValue_SomeMaxValue() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(MaxValue);

    }
}
