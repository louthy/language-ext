using static LanguageExt.Prelude;
using static LanguageExt.List;
using static LanguageExt.Queue;
using Xunit;
using LanguageExt;

namespace LanguageExt.Tests
{
    
    public class QueueTests
    {
        [Fact]
        public void EmptyQueuePeek()
        {
            var test = Queue<int>();
            var res = peek(test);

            Assert.True(res.IsNone);
        }

        [Fact]
        public void EmptyQueueDeq()
        {
            var test = Queue<int>();
            var res = map(deq(test), (stack, value) => value);

            Assert.True(res.IsNone);
        }

        [Fact]
        public void Dequeuing()
        {
            var test = Queue(1, 2, 3, 4, 5);
            Deq5(test);
        }

        [Fact]
        public void EnqDeq5()
        {
            var test = Queue<int>();

            test = enq(test, 1);
            test = enq(test, 2);
            test = enq(test, 3);
            test = enq(test, 4);
            test = enq(test, 5);

            Deq5(test);
        }

        void Deq5(Que<int> test)
        {
            test = map(deq(test), (queue, value) => { Assert.True(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.True(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.True(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.True(value.IsSome); return queue; });

            match(peek(test),
                Some: v => Assert.True(v == 5, "Actually equals "+v),
                None: () => Assert.False(true)
            );
        }

        [Fact]
        public void CollectionFunctions()
        {
            var queue = toQueue(Range(0,100));

            Assert.True(exists(queue, v => v == 50));
            Assert.True(length(queue) == 100);
            Assert.True(length(takeWhile(queue, v => v != 90)) == 90);
            Assert.True(length(take(queue, 10)) == 10);
            Assert.True(head(take(queue, 1)) == 0);
        }

        [Fact]
        public void RecursiveSumTest()
        {
            var values = toQueue(Range(1, 10));

            var res = Sum(values);

            Assert.True(res == sum(values));
        }

        public int Sum(Que<int> queue) =>
            map( deq(queue), (newqueue, option) =>
                match(option,
                    Some: value => value + Sum(newqueue),
                    None: ()    => 0
                )
            );
    }
}
