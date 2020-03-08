using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class TryOptionQue
    {
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryOptionFail<Que<int>>(new Exception("fail"));
            var mb = ma.Traverse(identity);
            var mc = Queue(TryOptionFail<int>(new Exception("fail")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<TryOption<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyQueIsQueSuccs()
        {
            var ma = TryOptionSucc(Queue(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Queue(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            Assert.True(mb == mc);
        }
    }
}
