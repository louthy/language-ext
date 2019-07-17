using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{

    public class OptionCoalesceTests
    {
        [Fact]
        public void OptionCoalesceTest1()
        {
            var optional = Some(123);
            var value = optional || 456;
            Assert.True(value == 123);
        }

        [Fact]
        public void OptionCoalesceTest2()
        {
            Option<int> optional = None;

            var value = optional || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void OptionCoalesceTest3()
        {
            Option<int> optional1 = None;
            Option<int> optional2 = None;
            var value = optional1 || optional2 || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void OptionUnsafeCoalesceTest1()
        {
            var optional = Some(123);
            var value = optional || 456;
            Assert.True(value == 123);
        }

        [Fact]
        public void OptionUnsafeCoalesceTest2()
        {
            OptionUnsafe<int> optional = None;

            var value = optional || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void OptionUnsafeCoalesceTest3()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || SomeUnsafe((string)null);
            Assert.True(value == SomeUnsafe((string)null));
        }

        [Fact]
        public void OptionUnsafeCoalesceTest3a()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || SomeUnsafe((string)null);
            Assert.True(value == null); 
        }

        [Fact]
        public void OptionUnsafeCoalesceTest3b()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || null;
            Assert.True(value == null);
        }

        [Fact]
        public void OptionUnsafeCoalesceTest3c()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || null;
            Assert.True(value == SomeUnsafe((string)null));
        }

        [Fact]
        public void OptionUnsafeCoalesceTest3d()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2;
            Assert.False(value == SomeUnsafe((string)null));
        }

        [Fact]
        public void OptionUnsafeCoalesceTest4()
        {
            OptionUnsafe<int> optional1 = None;
            OptionUnsafe<int> optional2 = None;
            var value = optional1 || optional2 || 456;
            Assert.True(value == 456);
        }

    }
}
