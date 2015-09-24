using static LanguageExt.Prelude;
using static LanguageExt.List;
using static LanguageExt.Queue;
using NUnit.Framework;
using LanguageExt;

namespace LanguageExtTests
{
    [TestFixture]
    public class QueueTests
    {
        [Test]
        public void EmptyQueuePeek()
        {
            var test = Queue<int>();
            var res = peek(test);

            Assert.IsTrue(res.IsNone);
        }

        [Test]
        public void EmptyQueueDeq()
        {
            var test = Queue<int>();
            var res = map(deq(test), (stack, value) => value);

            Assert.IsTrue(res.IsNone);
        }

        [Test]
        public void Dequeuing()
        {
            var test = Queue(1, 2, 3, 4, 5);
            Deq5(test);
        }

        [Test]
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

        public void Deq5(Que<int> test)
        {
            test = map(deq(test), (queue, value) => { Assert.IsTrue(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.IsTrue(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.IsTrue(value.IsSome); return queue; });
            test = map(deq(test), (queue, value) => { Assert.IsTrue(value.IsSome); return queue; });

            match(peek(test),
                Some: v => Assert.IsTrue(v == 5, "Actually equals "+v),
                None: () => Assert.Fail()
            );
        }

        [Test]
        public void CollectionFunctions()
        {
            var queue = toQueue(Range(0,100));

            Assert.IsTrue(exists(queue, v => v == 50));
            Assert.IsTrue(length(queue) == 100);
            Assert.IsTrue(length(takeWhile(queue, v => v != 90)) == 90);
            Assert.IsTrue(length(take(queue, 10)) == 10);
            Assert.IsTrue(head(take(queue, 1)) == 0);
        }

        [Test]
        public void RecursiveSumTest()
        {
            var values = toQueue(Range(1, 10));

            var res = Sum(values);

            Assert.IsTrue(res == sum(values));
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
