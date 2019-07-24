namespace LanguageExt.Tests.NullChecks
{
    public class notnullPreludeTests : AbstractNullCheckTests
    {
        protected override bool ExpectedWhenNull => false;
        protected override bool NullCheck<T>(T value) => Prelude.notnull(value);
    }
}
