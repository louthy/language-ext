using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class EitherLst
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = Left<Error, Lst<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = List(Left<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<Either<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptyLstIsLstRight()
        {
            var ma = Right<Error, Lst<int>>(List(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = List(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
