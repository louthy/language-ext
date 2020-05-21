using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Stck
    {
        [Fact]
        public void EmptyStckIsSuccessEmptyStck()
        {
            Stck<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Stck<string>>(Empty), mb);
        }

        [Fact]
        public void StckSuccessIsSuccessStck()
        {
            var ma = Stack(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Stck<int>>(Stack(2, 8, 64)), mb);
        }

        [Fact]
        public void StckSuccAndFailIsFailedStck()
        {
            var ma = Stack(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Stck<int>>(Error.New("failed")), mb);
        }
    }
}
