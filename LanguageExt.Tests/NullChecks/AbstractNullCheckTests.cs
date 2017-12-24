using Xunit;

namespace LanguageExt.Tests.NullChecks
{
    public abstract class AbstractNullCheckTests
    {
        protected abstract bool ExpectedWhenNull { get; }
        protected abstract bool NullCheck<T>(T value);

        protected bool ExpectedWhenNotNull => !ExpectedWhenNull;

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
        public void NullCheck_ZeroInt_AsExpectedWhenNotNull()
        {
            int value = 0;

            var actual = NullCheck(value);

            Assert.Equal(ExpectedWhenNotNull, actual);
        }

    }
}
