using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class EitherUnsafeSet
    {
        // TODO: OrdDefault
        // [Fact]
        // public void LeftIsSingletonLeft()
        // {
        //     var ma = LeftUnsafe<Error, Set<int>>(Error.New("alt"));
        //     var mb = ma.Sequence();
        //     var mc = Set(LeftUnsafe<Error, int>(new Exception("alt")));
        //
        //     Assert.True(mb == mc);
        // }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptySetIsSetRight()
        {
            var ma = RightUnsafe<Error, Set<int>>(Set(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Set(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3), RightUnsafe<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
