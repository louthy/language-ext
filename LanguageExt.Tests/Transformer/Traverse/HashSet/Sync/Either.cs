using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class EitherHashSet
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = Left<Error, HashSet<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = HashSet(Left<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = Right<Error, HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<Either<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptyHashSetIsHashSetRight()
        {
            var ma = Right<Error, HashSet<int>>(HashSet(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = HashSet(Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3), Right<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
