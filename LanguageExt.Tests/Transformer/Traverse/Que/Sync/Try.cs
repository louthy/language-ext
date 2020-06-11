using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class TryQue
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<Que<int>>(new Exception("fail"));
            var mb = ma.Traverse(identity);
            var mc = Queue(TryFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyQueIsQueSuccs()
        {
            var ma = TrySucc(Queue(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Queue(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
