using Xunit;

namespace LanguageExt.Tests.DefaultValueChecks
{
    public abstract class AbstractDefaultValueCheckTests
    {
        protected abstract bool ExpectedWhenDefaultValue { get; }
        protected abstract bool DefaultValueCheck<T>(T value);

        private bool ExpectedWhenNotDefaultValue => !ExpectedWhenDefaultValue;

        [Fact]
        public void DefaultValueCheck_DefaultValueString_AsExpectedWhenDefaultValue()
        {
            string value = null;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_HelloString_AsExpectedWhenNotDefaultValue()
        {
            string value = "hello";
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenNotDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_DefaultValueInt_AsExpectedWhenDefaultValue()
        {
            int value = 0;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_NonDefaultValueInt_AsExpectedWhenNotDefaultValue()
        {
            int value = 100;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenNotDefaultValue, actual);
        }
    }
}
