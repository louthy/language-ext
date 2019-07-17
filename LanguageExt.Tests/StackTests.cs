using static LanguageExt.Prelude;
using static LanguageExt.List;
using static LanguageExt.Stack;
using Xunit;
using LanguageExt;

namespace LanguageExt.Tests
{
    
    public class StackTests
    {
        [Fact]
        public void EmptyStackPeek()
        {
            var test = Stack<int>();
            var res = trypeek(test);

            Assert.True(res.IsNone);
        }

        [Fact]
        public void EmptyStackPop()
        {
            var test = Stack<int>();
            var res = map(trypop(test), (stack, value) => value);

            Assert.True(res.IsNone);
        }

        [Fact]
        public void Popping1()
        {
            var test = Stack(1, 2, 3, 4, 5);
            Popping5(test);
        }

        [Fact]
        public void Popping2()
        {
            var test = Stack<int>();

            test = push(test, 1);
            test = push(test, 2);
            test = push(test, 3);
            test = push(test, 4);
            test = push(test, 5);

            Popping5(test);
        }

        void Popping5(Stck<int> test)
        {
            test = map(trypop(test), (stack, value) => { Assert.True(value.IsSome); return stack; });
            test = map(trypop(test), (stack, value) => { Assert.True(value.IsSome); return stack; });
            test = map(trypop(test), (stack, value) => { Assert.True(value.IsSome); return stack; });
            test = map(trypop(test), (stack, value) => { Assert.True(value.IsSome); return stack; });
            peek(test,
                Some: v => Assert.True(v == 1),
                None: () => Assert.False(true)
            );
        }

        [Fact]
        public void CollectionFunctions()
        {
            var stack = toStack(Range(1,100));

            Assert.True(exists(stack, v => v == 50));
            Assert.True(length(stack) == 100);
            Assert.True(length(takeWhile(stack, v => v != 10)) == 90);
            Assert.True(length(take(stack, 10)) == 10);
            Assert.True(head(take(stack, 1)) == 100);
        }

        [Fact]
        public void RecursiveSumTest()
        {
            var values = toStack(Range(1, 10));

            var res = Sum(values);

            Assert.True(res == sum(values));
        }

        public int Sum(Stck<int> stack) =>
            pop(stack, 
                Some: (newstack, value) => value + Sum(newstack),
                None: ()                => 0
                );
    }
}
