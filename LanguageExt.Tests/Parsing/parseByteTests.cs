namespace LanguageExt.Tests.Parsing
{
    public class parseByteTests : AbstractParseTUnsignedPrecisionIntervalTests<byte>
    {
        protected override Option<byte> ParseT(string value) => Prelude.parseByte(value);

        protected override byte MinValue => byte.MinValue;
        protected override byte MaxValue => byte.MaxValue;
        protected override byte PositiveOne => 1;
        protected override byte PositiveTwo => 2;
    }
}
