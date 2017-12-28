namespace LanguageExt.Tests.Parsing
{
    public class parseULongTests : AbstractParseTUnsignedPrecisionIntervalTests<ulong>
    {
        protected override Option<ulong> ParseT(string value) => Prelude.parseULong(value);

        protected override ulong MinValue => ulong.MinValue;
        protected override ulong MaxValue => ulong.MaxValue;
        protected override ulong PositiveOne => 1;
        protected override ulong PositiveTwo => 2;
    }
}
