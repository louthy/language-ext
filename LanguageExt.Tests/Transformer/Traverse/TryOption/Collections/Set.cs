using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class SetTryOption
    {
        [Fact]
        public void EmptySetIsSuccEmptySet()
        {
            Set<TryOption<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Set<int>.Empty);

            Assert.True(default(EqTryOption<Set<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SetSuccsIsSuccSets()
        {
            var ma = Set(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Set(1, 2, 3));

            Assert.True(default(EqTryOption<Set<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SetSuccAndFailIsFail()
        {
            var ma = Set(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionFail<Set<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Set<int>>).Equals(mb, mc));
        }
    }
}
