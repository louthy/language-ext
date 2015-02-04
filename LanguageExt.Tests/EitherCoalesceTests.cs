using NUnit.Framework;
using LanguageExt;
using LanguageExt.Prelude;
using System;

namespace LanguageExtTests
{
    [TestFixture]
    class EitherCoalesceTests
    {
        [Test]
        public void EitherCoalesceTest1()
        {
            Either<int, string> either = 123;

            var value = either || 456;
            Assert.IsTrue(value == 123);
        }

        [Test]
        public void EitherCoalesceTest2()
        {
            Either<int, string> either = "Hello";

            var value = either || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void EitherCoalesceTest3()
        {
            Either<int, string> either1 = "Hello";
            Either<int, string> either2 = "World";

            var value = either1 || either2 || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void EitherUnsafeCoalesceTest1()
        {
            EitherUnsafe<int, string> either = 123;

            var value = either || 456;
            Assert.IsTrue(value == 123);
        }

        [Test]
        public void EitherUnsafeCoalesceTest2()
        {
            EitherUnsafe<int, string> either = "Hello";

            var value = either || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void EitherUnsafeCoalesceTest3()
        {
            EitherUnsafe<int, string> either1 = "Hello";
            EitherUnsafe<int, string> either2 = "World";

            var value = either1 || either2 || 456;
            Assert.IsTrue(value == 456);
        }

        [Test]
        public void EitherUnsafeCoalesceTest4()
        {
            EitherUnsafe<int, string> either1 = "Hello";
            EitherUnsafe<int, string> either2 = "World";
            EitherUnsafe<int, string> either3 = (string)null;

            var value = either1 || either2 || either3;
            Assert.IsTrue(value == null);
        }

        [Test]
        public void EitherUnsafeCoalesceTest5()
        {
            EitherUnsafe<string, int> either1 = 1;
            EitherUnsafe<string, int> either2 = 2;
            EitherUnsafe<string, int> either3 = 3;

            var value = either1 || either2 || either3 || (string)null;
            Assert.IsTrue(value == null);
        }
    }
}
