using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class IEnumerableTryOption
    {
        [Fact]
        public void EmptyIEnumerableIsSuccEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<TryOption<int>>();

            var mb = ma.Sequence();

            var mc = TryOption(Enumerable.Empty<int>());

            Assert.True(default(EqTryOption<IEnumerable<int>>).Equals(mb, mc));
        }

        [Fact]
        public void IEnumerableSuccIsSuccIEnumerables()
        {
            var ma = new[] { TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = TryOption(new[] { 1, 2, 3 }.AsEnumerable());
            Assert.True(default(EqTryOption<IEnumerable<int>>).Equals(mb, mc));
        }

        [Fact]
        public void IEnumerableSuccAndFailIsFail()
        {
            var ma = new[] { TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = TryOptionFail<IEnumerable<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<IEnumerable<int>>).Equals(mb, mc));
        }
    }
}
