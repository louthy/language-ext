using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Lst
    {
        [Fact]
        public void EmptyLstIsSuccessEmptyLst()
        {
            Lst<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Lst<string>>(Empty), mb);
        }

        [Fact]
        public void LstSuccessIsSuccessLst()
        {
            var ma = List(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Lst<int>>(List(2, 8, 64)), mb);
        }

        [Fact]
        public void LstFailedIsFailedLst()
        {
            var ma = List(Fail<Error, int>(Error.New("failed")), Fail<Error, int>(Error.New("failuire")));
            var mb = ma.Traverse(identity);
            var expected = Fail<Error, Lst<int>>(Seq(Error.New("failed"), Error.New("failuire")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void LstSuccAndFailIsFailedLst()
        {
            var ma = List(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Lst<int>>(Error.New("failed")), mb);
        }
    }
}
