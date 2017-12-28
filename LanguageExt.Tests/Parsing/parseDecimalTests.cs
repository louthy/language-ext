namespace LanguageExt.Tests.Parsing
{
    public class parseDecimalTests : AbstractParseTPrecisionIntervalTests<decimal>
    {
        protected override Option<decimal> ParseT(string value) => Prelude.parseDecimal(value);

        protected override decimal MinValue => decimal.MinValue;
        protected override decimal MaxValue => decimal.MaxValue;
    }
}
