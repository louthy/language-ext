using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class QueOptionUnsafe
    {
        [Fact]
        public void EmptyQueueIsSomeEmptyQueue()
        {
            Que<OptionUnsafe<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == SomeUnsafe(Que<int>.Empty));
        }
        
        [Fact]
        public void QueueSomesIsSomeQueues()
        {
            var ma = Queue(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == SomeUnsafe(Queue(1, 2, 3)));
        }
        
        [Fact]
        public void QueueSomeAndNoneIsNone()
        {
            var ma = Queue(SomeUnsafe(1), SomeUnsafe(2), None);

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == None);
        }
    }
}
