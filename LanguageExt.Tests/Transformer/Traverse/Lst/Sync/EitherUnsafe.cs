using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class EitherUnsafeLst
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, Lst<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = List(LeftUnsafe<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptyLstIsLstRight()
        {
            var ma = RightUnsafe<Error, Lst<int>>(List(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = List(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3), RightUnsafe<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
