using Xunit;

namespace LanguageExt.Tests
{

    public class StackTests
    {
        [Fact]
        public void EmptyStackPeek()
        {
            var test = Stck<int>();
            var res  = test.Peek();

            Assert.True(res.IsNone);
        }

        [Fact]
        public void EmptyStackPop()
        {
            var test = Stck<int>();
            var res  = test.Pop();
            Assert.True(res.IsEmpty);
        }

        [Fact]
        public void Popping1()
        {
            var test = Stck(1, 2, 3, 4, 5);
            Popping5(test);
        }

        [Fact]
        public void Popping2()
        {
            var test = Stck<int>();

            test = test.Push(1);
            test = test.Push(2);
            test = test.Push(3);
            test = test.Push(4);
            test = test.Push(5);

            Popping5(test);
        }

        void Popping5(Stck<int> test)
        {
            Assert.True(test.Peek().IsSome);
            test = test.Pop();

            Assert.True(test.Peek().IsSome);
            test = test.Pop();

            Assert.True(test.Peek().IsSome);
            test = test.Pop();

            Assert.True(test.Peek().IsSome);
            test = test.Pop();

            Assert.True(test.Peek() == 1);
        }
    }
}
