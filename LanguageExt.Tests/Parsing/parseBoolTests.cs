using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseBoolTests : AbstractParseTTests<bool>
    {
        protected override Option<bool> ParseT(string value) => Prelude.parseBool(value);

        [Fact]
        public void parseBool_ValidStringFromTrue_SomeTrue() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(true);
    }
}
