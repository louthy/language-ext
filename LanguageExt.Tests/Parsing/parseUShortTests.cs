namespace LanguageExt.Tests.Parsing
{
    public class parseUShortTests : AbstractParseTUnsignedPrecisionIntervalTests<ushort>
    {
        protected override Option<ushort> ParseT(string value) => Prelude.parseUShort(value);

        protected override ushort MinValue => ushort.MinValue;
        protected override ushort MaxValue => ushort.MaxValue;
        protected override ushort PositiveOne => 1;
        protected override ushort PositiveTwo => 2;
    }
}
