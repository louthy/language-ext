namespace LanguageExt.Tests.Parsing
{
    public class parseIntTests : AbstractParseTSignedPrecisionIntervalTests<int>
    {
        protected override Option<int> ParseT(string value) => Prelude.parseInt(value);

        protected override int MinValue => int.MinValue;
        protected override int MaxValue => int.MaxValue;
        protected override int NegativeOne => -1;
        protected override int PositiveOne => 1;
    }
}
