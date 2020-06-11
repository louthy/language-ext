using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class QueTry
    {
        [Fact]
        public void EmptyQueueIsSuccEmptyQueue()
        {
            Que<Try<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            var mc = TrySucc(Que<int>.Empty);
            
            Assert.True(default(EqTry<Que<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void QueueSuccsIsSuccQueues()
        {
            var ma = Queue(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TrySucc(Queue(1, 2, 3));
            
            Assert.True(default(EqTry<Que<int>>).Equals(mb, mc));

        }
        
        [Fact]
        public void QueueSuccAndFailIsFail()
        {
            var ma = Queue(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryFail<Que<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Que<int>>).Equals(mb, mc));
        }
    }
}
