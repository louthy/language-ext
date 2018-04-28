namespace LanguageExt.Tests.Parsing
{
    public class parseSByteTests : AbstractParseTSignedPrecisionIntervalTests<sbyte>
    {
        protected override Option<sbyte> ParseT(string value) => Prelude.parseSByte(value);

        protected override sbyte MinValue => sbyte.MinValue;
        protected override sbyte MaxValue => sbyte.MaxValue;
        protected override sbyte NegativeOne => -1;
        protected override sbyte PositiveOne => 1;
    }
}
