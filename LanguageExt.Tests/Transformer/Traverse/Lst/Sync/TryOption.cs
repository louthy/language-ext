using System;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class TryOptionLst
    {
        
        private readonly ITestOutputHelper _log;
        public TryOptionLst(ITestOutputHelper log) => _log = log;
        
        [Fact]
        public void FailIsSingletonNone()
        {
            var ma = TryOptionFail<Lst<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = List(TryOptionFail<int>(new Exception("fail")));

            _log.WriteLine(mb.Map(t => t.AsString()).ToFullString());
            _log.WriteLine(mc.Map(t => t.AsString()).ToFullString());
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<TryOption<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccNonEmptyLstIsLstSuccs()
        {
            var ma = TryOptionSucc(List(1, 2, 3));
            var mb = ma.Sequence();
            var mc = List(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));
            
            _log.WriteLine(mb.Map(t => t.AsString()).ToFullString());
            _log.WriteLine(mc.Map(t => t.AsString()).ToFullString());

            Assert.True(mb == mc);
        }
    }
}
