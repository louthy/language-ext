using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class SetTry
    {
        [Fact]
        public void EmptySetIsSuccEmptySet()
        {
            Set<Try<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TrySucc(Set<int>.Empty);
            
            Assert.True(default(EqTry<Set<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SetSuccsIsSuccSets()
        {
            var ma = Set(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Sequence();

            var mc = TrySucc(Set(1, 2, 3));

            Assert.True(default(EqTry<Set<int>>).Equals(mb, mc));

        }

        [Fact]
        public void SetSuccAndFailIsFail()
        {
            var ma = Set(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryFail<Set<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Set<int>>).Equals(mb, mc));
        }
    }
}
