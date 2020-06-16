using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Validation.Collections
{
    public class IEnumerable
    {
        [Fact]
        public void EmptyIEnumerableIsSuccessIEnumerable()
        {
            var ma = Enumerable.Empty<Validation<Error, int>>();
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, IEnumerable<int>>(LanguageExt.Seq.empty<int>()), mb);
        }

        [Fact]
        public void IEnumerableSuccessIsSuccessIEnumerable()
        {
            var ma = List(Success<Error, int>(2), Success<Error, int>(8), Success<Error, int>(64))
                .AsEnumerable();
            var mb = ma.Traverse(identity);
            Assert.Equal(Success<Error, IEnumerable<int>>(List(2, 8, 64).AsEnumerable()), mb);
        }

        [Fact]
        public void IEnumerableSuccAndFailIsFailedIEnumerable()
        {
            var ma = List(Fail<Error, int>(Error.New("failed")), Success<Error, int>(12)).AsEnumerable();
            var mb = ma.Traverse(identity);
            Assert.Equal(Fail<Error, IEnumerable<int>>(Error.New("failed")), mb);
        }
    }
}
