using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class TryOptionOption
    {
        [Fact]
        public void FailIsSomeFail()
        {
            var ma = TryOption<Option<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = Some(TryOption<int>(new Exception("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccNoneIsNone()
        {
            var ma = TryOption<Option<int>>(None);
            var mb = ma.Sequence();
            var mc = Option<TryOption<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccSomeIsSomeSucc()
        {
            var ma = TryOption<Option<int>>(Some(1234));
            var mb = ma.Sequence();
            var mc = Some(TryOption(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
