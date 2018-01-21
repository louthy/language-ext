namespace LanguageExt.Tests.DefaultValueChecks
{
    public class notDefaultPreludeTests : AbstractDefaultValueCheckTests
    {
        protected override bool ExpectedWhenDefaultValue => false;
        protected override bool DefaultValueCheck<T>(T value) => Prelude.notDefault(value);
    }
}
