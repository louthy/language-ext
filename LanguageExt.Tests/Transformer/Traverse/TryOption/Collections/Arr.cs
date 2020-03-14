using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class ArrTryOption

    {
        [Fact]
        public void EmptyArrIsSuccEmptyArr()
        {
            Arr<TryOption<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Arr<int>.Empty);

            Assert.True(default(EqTryOption<Arr<int>>).Equals(mb, mc));
        }

        [Fact]
        public void ArrSuccsIsSuccArrs()
        {
            var ma = Array(TryOption(1), TryOption(2), TryOption(3));

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Array(1, 2, 3));

            Assert.True(default(EqTryOption<Arr<int>>).Equals(mb, mc));
        }

        [Fact]
        public void ArrSuccAndFailIsFail()
        {
            var ma = Array(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionFail<Arr<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Arr<int>>).Equals(mb, mc));
        }
    }
}
