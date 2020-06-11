using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Sync
{
    public class OptionUnsafe
    {
        [Fact]
        public void NoneIsSuccessNone()
        {
            var ma = OptionUnsafe<Validation<Error, int>>.None;
            var mb = ma.Sequence();
            var mc = Success<Error, OptionUnsafe<int>>(None);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeSuccessIsSuccessSome()
        {
            var ma = SomeUnsafe(Success<Error, int>(12));
            var mb = ma.Sequence();
            var mc = Success<Error, OptionUnsafe<int>>(12);

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SomeFailIsFailSome()
        {
            var ma = SomeUnsafe(Fail<Error, int>(Error.New("Err")));
            var mb = ma.Sequence();
            var mc = Fail<Error, OptionUnsafe<int>>(Error.New("Err"));

            Assert.Equal(mc, mb);
        }
    }
}
