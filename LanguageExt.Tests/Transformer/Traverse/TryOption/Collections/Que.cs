using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class QueTryOption

    {
        [Fact]
        public void EmptyQueueIsSuccEmptyQueue()
        {
            Que<TryOption<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = TryOptionSucc(Que<int>.Empty);

            Assert.True(default(EqTryOption<Que<int>>).Equals(mb, mc));
        }

        [Fact]
        public void QueueSuccsIsSuccQueues()
        {
            var ma = Queue(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Traverse(identity);

            var mc = TryOptionSucc(Queue(1, 2, 3));

            Assert.True(default(EqTryOption<Que<int>>).Equals(mb, mc));
        }

        [Fact]
        public void QueueSuccAndFailIsFail()
        {
            var ma = Queue(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Traverse(identity);

            var mc = TryOptionFail<Que<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Que<int>>).Equals(mb, mc));
        }
    }
}
