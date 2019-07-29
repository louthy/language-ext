using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class EitherCoalesceTests
    {
        [Fact]
        public void EitherCoalesceTest1()
        {
            Either<string, int> either = 123;

            var value = either || 456;
            Assert.True(value == 123);
        }

        [Fact]
        public void EitherCoalesceTest2()
        {
            Either<string, int> either = "Hello";

            var value = either || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void EitherCoalesceTest3()
        {
            Either<string, int> either1 = "Hello";
            Either<string, int> either2 = "World";

            var value = either1 || either2 || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void EitherUnsafeCoalesceTest1()
        {
            EitherUnsafe<string, int> either = 123;

            var value = either || 456;
            Assert.True(value == 123);
        }

        [Fact]
        public void EitherUnsafeCoalesceTest2()
        {
            EitherUnsafe<string, int> either = "Hello";

            var value = either || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void EitherUnsafeCoalesceTest3()
        {
            EitherUnsafe<string, int> either1 = "Hello";
            EitherUnsafe<string, int> either2 = "World";

            var value = either1 || either2 || 456;
            Assert.True(value == 456);
        }

        [Fact]
        public void EitherUnsafeCoalesceTest4()
        {
            EitherUnsafe<string, int> either1 = "Hello";
            EitherUnsafe<string, int> either2 = "World";
            EitherUnsafe<string, int> either3 = (string)null;

            var value = either1 || either2 || either3;
            Assert.True(value == null);
        }

        [Fact]
        public void EitherUnsafeCoalesceTest5()
        {
            EitherUnsafe<int, string> either1 = 1;
            EitherUnsafe<int, string> either2 = 2;
            EitherUnsafe<int, string> either3 = 3;

            var value = either1 || either2 || either3 || (string)null;
            Assert.True(value == null);
        }

        [Fact]
        public void TryFun()
        {
            var x = TryOption(() => 100);
        }
    }
}
