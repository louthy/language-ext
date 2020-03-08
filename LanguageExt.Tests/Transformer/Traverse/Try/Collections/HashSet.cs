using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class HashSetTry
    {
        [Fact]
        public void EmptyHashSetIsSomeEmptyHashSet()
        {
            HashSet<Try<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TrySucc(HashSet<int>.Empty);
            
            Assert.True(default(EqTry<HashSet<int>>).Equals(mb, mc));
            
        }
        
        [Fact]
        public void HashSetSomesIsSomeHashSets()
        {
            var ma = HashSet(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Sequence();

            var mc = TrySucc(HashSet(1, 2, 3));
            
            Assert.True(default(EqTry<HashSet<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void HashSetSomeAndNoneIsNone()
        {
            var ma = HashSet(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryFail<HashSet<int>>(new Exception("fail"));

            Assert.True(default(EqTry<HashSet<int>>).Equals(mb, mc));
        }
    }
}
