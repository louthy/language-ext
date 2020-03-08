using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Sync
{
    public class ValidationSeqOption
    {
        [Fact]
        public void FailNoneIsNone()
        {
            var ma = Fail<Error, Option<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = Option<Validation<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessNoneIsNone()
        {
            var ma = Success<Error, Option<int>>(None);
            var mb = ma.Sequence();
            var mc = Option<Validation<Error, int>>.None;

            var mr = mb == mc;
            
            Assert.True(mr);
        }
        
        [Fact]
        public void SuccessSomeIsSomeSuccess()
        {
            var ma = Success<Error, Option<int>>(Some(1234));
            var mb = ma.Sequence();
            var mc = Some(Success<Error, int>(1234));

            var mr = mb == mc;
            
            Assert.True(mr);
        }
    }
}
