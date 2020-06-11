using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class LstTryOption
    {
        [Fact]
        public void EmptyLstIsSuccEmptyLst()
        {
            Lst<TryOption<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Lst<int>.Empty);

            Assert.True(default(EqTryOption<Lst<int>>).Equals(mb, mc));
        }

        [Fact]
        public void LstSuccsIsSuccLsts()
        {
            var ma = List(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Sequence();

            var mc = TryOptionSucc(List(1, 2, 3));

            Assert.True(default(EqTryOption<Lst<int>>).Equals(mb, mc));
        }

        [Fact]
        public void LstSuccAndFailIsFail()
        {
            var ma = List(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionFail<Lst<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Lst<int>>).Equals(mb, mc));
        }
    }
}
