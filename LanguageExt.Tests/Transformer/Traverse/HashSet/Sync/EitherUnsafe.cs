using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.HashSetT.Sync
{
    public class EitherUnsafeHashSet
    {
        [Fact]
        public void LeftIsSingletonLeft()
        {
            var ma = LeftUnsafe<Error, HashSet<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = HashSet(LeftUnsafe<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightEmptyIsEmpty()
        {
            var ma = RightUnsafe<Error, HashSet<int>>(Empty);
            var mb = ma.Sequence();
            var mc = HashSet<EitherUnsafe<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void RightNonEmptyHashSetIsHashSetRight()
        {
            var ma = RightUnsafe<Error, HashSet<int>>(HashSet(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = HashSet(RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3), RightUnsafe<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
