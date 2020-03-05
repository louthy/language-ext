using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafe.Sync
{
    public class TryOptionUnsafe
    {
        [Fact]
        public void FailIsNone()
        {
            var ma = Try<OptionUnsafe<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = OptionUnsafe<Try<int>>.None;

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
            var ma = Try(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Try(1234));

            var mr = (from tb in mb
                      from tc in mc
                      select tb.Try().Equals(tc.Try())).IfNoneUnsafe(false);
            
            Assert.True(mr);
        }
    }
}
