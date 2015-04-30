using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    class OptionCoalesceTests
    {
        [Test]
        public void OptionCoalesceTest1()
        {
            var optional = Some(123);
            var value = optional || 456;
            Assert.IsTrue(value == 123);
        }

        [Test]
        public void OptionCoalesceTest2()
        {
            Option<int> optional = None;

            var value = optional || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void OptionCoalesceTest3()
        {
            Option<int> optional1 = None;
            Option<int> optional2 = None;
            var value = optional1 || optional2 || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void OptionUnsafeCoalesceTest1()
        {
            var optional = Some(123);
            var value = optional || 456;
            Assert.IsTrue(value == 123);
        }

        [Test]
        public void OptionUnsafeCoalesceTest2()
        {
            OptionUnsafe<int> optional = None;

            var value = optional || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void OptionUnsafeCoalesceTest3()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || SomeUnsafe((string)null);
            Assert.IsTrue(value == SomeUnsafe((string)null));
        }

        [Test]
        public void OptionUnsafeCoalesceTest3a()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || SomeUnsafe((string)null);
            Assert.IsTrue(value == null); 
        }

        [Test]
        public void OptionUnsafeCoalesceTest3b()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || null;
            Assert.IsTrue(value == null);
        }

        [Test]
        public void OptionUnsafeCoalesceTest3c()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || null;
            Assert.IsTrue(value == SomeUnsafe((string)null));
        }

        [Test]
        public void OptionUnsafeCoalesceTest3d()
        {
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2;
            Assert.IsFalse(value == SomeUnsafe((string)null));
        }

        [Test]
        public void OptionUnsafeCoalesceTest4()
        {
            OptionUnsafe<int> optional1 = None;
            OptionUnsafe<int> optional2 = None;
            var value = optional1 || optional2 || 456;
            Assert.IsTrue(value == 456);
        }

    }
}
