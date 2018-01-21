namespace LanguageExt.Tests.Parsing
{
    public class parseLongTests : AbstractParseTSignedPrecisionIntervalTests<long>
    {
        protected override Option<long> ParseT(string value) => Prelude.parseLong(value);

        protected override long MinValue => long.MinValue;
        protected override long MaxValue => long.MaxValue;
        protected override long NegativeOne => -1;
        protected override long PositiveOne => 1;
    }
}
