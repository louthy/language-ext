using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class TryOptionOptionUnsafe
    {
        [Fact]
        public void FailIsSomeFail()
        {
            var ma = TryOption<OptionUnsafe<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(TryOption<int>(new Exception("fail")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccNoneIsNone()
        {
            var ma = TryOption<OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<TryOption<int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccSomeIsSomeSucc()
        {
            var ma = TryOption<OptionUnsafe<int>>(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(TryOption(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
