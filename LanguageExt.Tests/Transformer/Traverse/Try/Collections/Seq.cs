using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class SeqTry
    {
        [Fact]
        public void EmptySeqIsSuccEmptySeq()
        {
            Seq<Try<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TrySucc(Seq<int>.Empty);
            
            Assert.True(default(EqTry<Seq<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void SeqSuccsIsSuccSeqs()
        {
            var ma = Seq(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Sequence();

            var mc = TrySucc(Seq(1, 2, 3));
            
            Assert.True(default(EqTry<Seq<int>>).Equals(mb, mc));

        }
        
        [Fact]
        public void SeqSuccAndFailIsFail()
        {
            var ma = Seq(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryFail<Seq<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Seq<int>>).Equals(mb, mc));
        }
    }
}
