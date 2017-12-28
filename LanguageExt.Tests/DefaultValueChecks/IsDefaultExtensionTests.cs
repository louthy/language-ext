namespace LanguageExt.Tests.DefaultValueChecks
{
    public class IsDefaultExtensionTests : AbstractDefaultValueCheckTests
    {
        protected override bool ExpectedWhenDefaultValue => true;
        protected override bool DefaultValueCheck<T>(T value) => value.IsDefault();
    }
}
