using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class TryOptionSet
    {
        // TODO: OrdDefault
        // [Fact]
        // public void FailIsSingletonNone()
        // {
        //     var ma = TryOptionFail<Set<int>>(new Exception("fail"));
        //     var mb = ma.Sequence();
        //     var mc = Set(TryOptionFail<int>(new Exception("fail")));
        //     
        //     Assert.True(mb == mc);
        // }
        
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<TryOption<int>>();

            Assert.True(mb == mc);
        }

        // TODO: OrdDefault
        // [Fact]
        // public void SuccNonEmptySetIsSetSuccs()
        // {
        //     var ma = TryOptionSucc(Set(1, 2, 3));
        //     var mb = ma.Sequence();
        //     var mc = Set(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));
        //
        //     Assert.True(mb == mc);
        // }
    }
}
