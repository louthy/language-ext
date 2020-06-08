using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class Option
    {
        [Fact]
        public void NoneIsSuccessNone()
        {
            var ma = Option<Validation<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = Success<Error, Option<int>>(None);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeSuccessIsSuccessSome()
        {
            var ma = Some(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, Option<int>>(12);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeFailIsFailSome()
        {
            var ma = Some(Fail<Error, int>(Error.New("Err")));
            var mb = ma.Sequence();
            var mc = Fail<Error, Option<int>>(Error.New("Err"));

            Assert.Equal(mc, mb);
        }
    }
}
