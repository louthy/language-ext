using LanguageExt;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class QueOption
    {
        [Fact]
        public void EmptyQueueIsSomeEmptyQueue()
        {
            Que<Option<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Some(Que<int>.Empty));
        }
        
        [Fact]
        public void QueueSomesIsSomeQueues()
        {
            var ma = Queue(Some(1), Some(2), Some(3));

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == Some(Queue(1, 2, 3)));
        }
        
        [Fact]
        public void QueueSomeAndNoneIsNone()
        {
            var ma = Queue(Some(1), Some(2), None);

            var mb = ma.Traverse(Prelude.identity);

            Assert.True(mb == None);
        }
    }
}
