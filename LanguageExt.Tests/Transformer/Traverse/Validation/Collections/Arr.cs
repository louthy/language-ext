using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Arr
    {
        [Fact]
        public void EmptyArrayIsSuccessEmptyArray()
        {
            Arr<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Arr<string>>(Empty), mb);
        }

        [Fact]
        public void ArraySuccessIsSuccessArray()
        {
            var ma = Array(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Arr<int>>(Array(2, 8, 64)), mb);
        }

        [Fact]
        public void ArrayFailedIsFailedArray()
        {
            var ma = Array(Fail<Error, int>(Error.New("failed")), Fail<Error, int>(Error.New("failuire")));
            var mb = ma.Traverse(identity);
            var expected = Fail<Error, Arr<int>>(Seq(Error.New("failed"), Error.New("failuire")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void ArrSuccAndFailIsFailedArr()
        {
            var ma = Array(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Arr<int>>(Error.New("failed")), mb);
        }
    }
}
