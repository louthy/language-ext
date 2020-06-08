using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class Set
    {
        [Fact]
        public void EmptySetIsSuccessEmptySet()
        {
            Set<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Set<string>>(Empty), mb);
        }

        [Fact]
        public void SetSuccessIsSuccessSet()
        {
            var ma = Set(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, Set<int>>(Set(2, 8, 64)), mb);
        }

        [Fact]
        public void SetSuccAndFailIsFailedSet()
        {
            var ma = Set(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, Set<int>>(Error.New("failed")), mb);
        }
    }
}
