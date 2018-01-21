namespace LanguageExt.Tests.NullChecks
{
    public class isnullPreludeTests : AbstractNullCheckTests
    {
        protected override bool ExpectedWhenNull => true;
        protected override bool NullCheck<T>(T value) => Prelude.isnull(value);
    }
}
