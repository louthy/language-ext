using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class HashSetTryOption
    {
        [Fact]
        public void EmptyHashSetIsSuccEmptyHashSet()
        {
            HashSet<TryOption<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TryOptionSucc(HashSet<int>.Empty);

            Assert.True(default(EqTryOption<HashSet<int>>).Equals(mb, mc));
        }

        [Fact]
        public void HashSetSuccsIsSuccHashSets()
        {
            var ma = HashSet(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Sequence();

            var mc = TryOptionSucc(HashSet(1, 2, 3));

            Assert.True(default(EqTryOption<HashSet<int>>).Equals(mb, mc));
        }

        [Fact]
        public void HashSetSuccsAndFailIsNone()
        {
            var ma = HashSet(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionFail<HashSet<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<HashSet<int>>).Equals(mb, mc));
        }
    }
}
