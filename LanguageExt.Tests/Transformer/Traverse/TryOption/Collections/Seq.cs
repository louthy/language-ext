using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class SeqTryOption
    {
        [Fact]
        public void EmptySeqIsSuccEmptySeq()
        {
            Seq<TryOption<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Seq<int>.Empty);

            Assert.True(default(EqTryOption<Seq<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SeqSuccsIsSuccSeqs()
        {
            var ma = Seq(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Sequence();

            var mc = TryOptionSucc(Seq(1, 2, 3));

            Assert.True(default(EqTryOption<Seq<int>>).Equals(mb, mc));
        }

        [Fact]
        public void SeqSuccAndFailIsFail()
        {
            var ma = Seq(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryOptionFail<Seq<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Seq<int>>).Equals(mb, mc));
        }
    }
}
