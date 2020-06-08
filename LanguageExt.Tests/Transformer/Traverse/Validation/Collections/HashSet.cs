using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class HashSet
    {
        [Fact]
        public void EmptyHashSetIsSuccessEmptyHashSet()
        {
            HashSet<Validation<Error, string>> ma = Empty;
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, HashSet<string>>(Empty), mb);
        }

        [Fact]
        public void HashSetSuccessIsSuccessHashSet()
        {
            var ma = HashSet(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64));
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, HashSet<int>>(HashSet(2, 8, 64)), mb);
        }

        [Fact]
        public void HashSetSuccAndFailIsFailedHashSet()
        {
            var ma = HashSet(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12));
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, HashSet<int>>(Error.New("failed")), mb);
        }
    }
}
