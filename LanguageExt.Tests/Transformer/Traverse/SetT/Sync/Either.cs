using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class EitherSet
    {
        // TODO: OrdDefault
        // [Fact]
        // public void LeftIsSingletonLeft()
        // {
        //     var ma = Left<Error, Set<int>>(Error.New("alt"));
        //     var mb = ma.Sequence();
        //     var mc = Set(Left<Error, int>(new Exception("alt")));
        //
        //     Assert.True(mb == mc);
        // }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<Either<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptySetIsSetRight()
        {
            var ma = Right<Error, Set<int>>(Set(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Set(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
