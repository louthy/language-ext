using static LanguageExt.Prelude;
using static LanguageExt.List;
using static LanguageExt.Stack;
using NUnit.Framework;
using System.Collections.Immutable;

namespace LanguageExtTests
{
    [TestFixture]
    public class StackTests
    {
        [Test]
        public void EmptyStackPeek()
        {
            var test = stack<int>();
            var res = peek(test);

            Assert.IsTrue(res.IsNone);
        }

        [Test]
        public void EmptyStackPop()
        {
            var test = stack<int>();
            var res = map(pop(test), (stack, value) => value);

            Assert.IsTrue(res.IsNone);
        }

        [Test]
        public void Popping1()
        {
            var test = stack<int>(1, 2, 3, 4, 5);
            Popping5(test);
        }

        [Test]
        public void Popping2()
        {
            var test = stack<int>();

            test = push(test, 1);
            test = push(test, 2);
            test = push(test, 3);
            test = push(test, 4);
            test = push(test, 5);

            Popping5(test);
        }

        public void Popping5(IImmutableStack<int> test)
        {
            test = map(pop(test), (stack, value) => { Assert.IsTrue(value.IsSome); return stack; });
            test = map(pop(test), (stack, value) => { Assert.IsTrue(value.IsSome); return stack; });
            test = map(pop(test), (stack, value) => { Assert.IsTrue(value.IsSome); return stack; });
            test = map(pop(test), (stack, value) => { Assert.IsTrue(value.IsSome); return stack; });
            match(peek(test),
                Some: v => Assert.IsTrue(v == 1),
                None: () => Assert.Fail()
            );
        }

        [Test]
        public void CollectionFunctions()
        {
            var stack = toStack(range(1,100));

            Assert.IsTrue(exists(stack, v => v == 50));
            Assert.IsTrue(length(stack) == 100);
            Assert.IsTrue(length(takeWhile(stack, v => v != 10)) == 90);
            Assert.IsTrue(length(take(stack, 10)) == 10);
            Assert.IsTrue(head(take(stack, 1)) == 100);
        }

        [Test]
        public void RecursiveSumTest()
        {
            var values = toStack(range(1, 10));

            var res = Sum(values);

            Assert.IsTrue(res == sum(values));
        }

        public int Sum(IImmutableStack<int> stack) =>
            map( pop(stack), (newstack, option) =>
                match(option,
                    Some: value => value + Sum(newstack),
                    None: ()    => 0
                )
            );
    }
}
