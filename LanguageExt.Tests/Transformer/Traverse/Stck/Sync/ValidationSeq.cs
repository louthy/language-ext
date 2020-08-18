using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class ValidationSeq
    {
        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<Validation<Error, int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SuccessNonEmptyIsStackOfSuccess()
        {
            var ma = Success<Error, Stck<int>>(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void FailIsStackFail()
        {
            var ma = Fail<Error, Stck<int>>(Error.New("error"));
            var mb = ma.Traverse(identity);
            var mc = Stack(Fail<Error, int>(Error.New("error")));

            Assert.Equal(mc, mb);
        }
    }
}
