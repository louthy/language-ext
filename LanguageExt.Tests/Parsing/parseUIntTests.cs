namespace LanguageExt.Tests.Parsing
{
    public class parseUIntTests : AbstractParseTUnsignedPrecisionIntervalTests<uint>
    {
        protected override Option<uint> ParseT(string value) => Prelude.parseUInt(value);

        protected override uint MinValue => uint.MinValue;
        protected override uint MaxValue => uint.MaxValue;
        protected override uint PositiveOne => 1;
        protected override uint PositiveTwo => 2;
    }
}
