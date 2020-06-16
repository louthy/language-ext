    using LanguageExt.Common;
    using Xunit;
    using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Que
    {
        [Fact]
        public void EmptyQueIsSuccessEmptyQue()
        {
            Que<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Que<string>>(Empty), mb);
        }

        [Fact]
        public void QueSuccessIsSuccessQue()
        {
            var ma = Queue(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Que<int>>(Queue(2, 8, 64)), mb);
        }

        [Fact]
        public void QueFailedIsFailedQue()
        {
            var ma = Queue(Fail<Error, int>(Error.New("failed")), Fail<Error, int>(Error.New("failuire")));
            var mb = ma.Traverse(identity);
            var expected = Fail<Error, Que<int>>(Seq(Error.New("failed"), Error.New("failuire")));
            Assert.Equal(expected, mb);
        }

        [Fact]
        public void QueSuccAndFailIsFailedQue()
        {
            var ma = Queue(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Que<int>>(Error.New("failed")), mb);
        }
    }
}
