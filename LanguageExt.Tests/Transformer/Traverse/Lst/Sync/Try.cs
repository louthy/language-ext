using System;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class TryLst
    {
        private readonly ITestOutputHelper _log;
        public TryLst(ITestOutputHelper log) => _log = log;
        
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryFail<Lst<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = List(TryFail<int>(new Exception("fail")));

            _log.WriteLine(mb.Map(t => t.AsString()).ToFullString());
            _log.WriteLine(mc.Map(t => t.AsString()).ToFullString());
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<Try<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyLstIsHashSetSuccs()
        {
            var ma = TrySucc(HashSet(1, 2, 3));
            var mb = ma.Sequence();
            var mc = HashSet(TrySucc(1), TrySucc(2), TrySucc(3));

            Assert.True(mb == mc);
        }
    }
}
