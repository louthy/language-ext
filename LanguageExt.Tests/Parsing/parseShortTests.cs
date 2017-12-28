namespace LanguageExt.Tests.Parsing
{
    public class parseShortTests : AbstractParseTSignedPrecisionIntervalTests<short>
    {
        protected override Option<short> ParseT(string value) => Prelude.parseShort(value);

        protected override short MinValue => short.MinValue;
        protected override short MaxValue => short.MaxValue;
        protected override short NegativeOne => -1;
        protected override short PositiveOne => 1;
    }
}
