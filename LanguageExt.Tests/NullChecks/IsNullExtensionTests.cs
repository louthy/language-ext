namespace LanguageExt.Tests.NullChecks
{
    public class IsNullExtensionTests : AbstractNullCheckTests
    {
        protected override bool ExpectedWhenNull => true;
        protected override bool NullCheck<T>(T value) => value.IsNull();
    }
}
