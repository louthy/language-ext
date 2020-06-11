using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Sync
{
    public class ValidationSeqOptionUnsafe
    {
        [Fact]
        public void FailIsSomeFail()
        {
            var ma = Fail<Error, OptionUnsafe<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Fail<Error, int>(Error.New("alt")));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessNoneIsNone()
        {
            var ma = Success<Error, OptionUnsafe<int>>(None);
            var mb = ma.Sequence();
            var mc = OptionUnsafe<Validation<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessSomeIsSomeSuccess()
        {
            var ma = Success<Error, OptionUnsafe<int>>(SomeUnsafe(1234));
            var mb = ma.Sequence();
            var mc = SomeUnsafe(Success<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
