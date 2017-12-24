using Xunit;

namespace LanguageExt.Tests.NullChecks
{
    public abstract class AbstractNullCheckTests
    {
        protected abstract bool ExpectedWhenNull { get; }
        protected abstract bool NullCheck<T>(T value);

        private bool ExpectedWhenNotNull => !ExpectedWhenNull;

        [Fact]
        public void NullCheck_NullObject_AsExpectedWhenNull()
        {
            object value = null;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNull, actual);
        }

        [Fact]
        public void NullCheck_NonNullObject_AsExpectedWhenNotNull()
        {
            object value = new object();
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_NullString_AsExpectedWhenNull()
        {
            string value = null;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNull, actual);
        }

        [Fact]
        public void NullCheck_HelloString_AsExpectedWhenNotNull()
        {
            string value = "hello";
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_NullCustomClass_AsExpectedWhenNull()
        {
            FooClass value = null;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNull, actual);
        }

        [Fact]
        public void NullCheck_DefaultConstructorCustomClass_AsExpectedWhenNotNull()
        {
            FooClass value = new FooClass();
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_NullNullableByte_AsExpectedWhenNull()
        {
            byte? value = null;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNull, actual);
        }

        [Fact]
        public void NullCheck_ZeroNullableByte_AsExpectedWhenNotNull()
        {
            byte? value = 0;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_ZeroInt_AsExpectedWhenNotNull()
        {
            int value = 0;
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_DefaultCustomEnum_AsExpectedWhenNull()
        {
            FooEnum value = default(FooEnum);
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        [Fact]
        public void NullCheck_DefaultConstructorCustomStruct_AsExpectedWhenNull()
        {
            FooStruct value = new FooStruct();
            var actual = NullCheck(value);
            Assert.Equal(ExpectedWhenNotNull, actual);
        }

        private class FooClass { }
        private enum FooEnum { }
        private struct FooStruct { }

    }
}
