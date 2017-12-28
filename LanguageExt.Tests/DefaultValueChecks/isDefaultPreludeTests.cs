namespace LanguageExt.Tests.DefaultValueChecks
{
    public class isDefaultPreludeTests : AbstractDefaultValueCheckTests
    {
        protected override bool ExpectedWhenDefaultValue => true;
        protected override bool DefaultValueCheck<T>(T value) => Prelude.isDefault(value);
    }
}
