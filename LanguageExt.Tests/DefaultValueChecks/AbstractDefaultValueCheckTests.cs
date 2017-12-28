using Xunit;

namespace LanguageExt.Tests.DefaultValueChecks
{
    public abstract class AbstractDefaultValueCheckTests
    {
        protected abstract bool ExpectedWhenDefaultValue { get; }
        protected abstract bool DefaultValueCheck<T>(T value);

        private bool ExpectedWhenNotDefaultValue => !ExpectedWhenDefaultValue;

        [Fact]
        public void DefaultValueCheck_DefaultValueObject_AsExpectedWhenDefaultValue()
        {
            object value = null;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_DefaultConstructorObject_AsExpectedWhenNotDefaultValue()
        {
            object value = new object();
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenNotDefaultValue, actual);
        }

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

        [Fact]
        public void DefaultValueCheck_DefaultValueNullableByte_AsExpectedWhenDefaultValue()
        {
            byte? value = null;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_ZeroNullableByte_AsExpectedWhenNotDefaultValue()
        {
            byte? value = 0;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenNotDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_DefaultConstructorEnum_AsExpectedWhenDefaultValue()
        {
            FooEnum value = new FooEnum();
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenDefaultValue, actual);
        }

        [Fact]
        public void DefaultValueCheck_FirstEnumOptionWithValueOne_AsExpectedWhenNotDefaultValue()
        {
            FooEnum value = FooEnum.FooEnumState;
            var actual = DefaultValueCheck(value);
            Assert.Equal(ExpectedWhenNotDefaultValue, actual);
        }

        private enum FooEnum
        {
            FooEnumState = 1
        }
    }
}
