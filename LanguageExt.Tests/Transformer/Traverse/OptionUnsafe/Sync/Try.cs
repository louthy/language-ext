using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class TryOptionUnsafe
    {
        [Fact]
        public void FailIsSomeFail()
        {
            var ma = Try<OptionUnsafe<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Try<int>(new Exception("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccNoneIsNone()
        {
            var ma = Try<OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<Try<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccSomeIsSomeSucc()
        {
            var ma = TrySucc(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(TrySucc(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
