using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherT.Collections
{
    public class IEnumerableEither
    {
        [Fact]
        public void EmptyIEnumerableIsRightEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<Either<Error, int>>();

            var mb = ma.Sequence();

            var mr = mb.Map(b => ma.Count() == b.Count())
                       .IfLeft(false);
            
            Assert.True(mr);
        }

        [Fact]
        public void IEnumerableRightsIsRightIEnumerables()
        {
            var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Right<Error, int>(3)}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb.Map(b => default(EqEnumerable<int>).Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfLeft(false));
        }

        [Fact]
        public void IEnumerableRightAndLeftIsLeftEmpty()
        {
            var ma = new[] {Right<Error, int>(1), Right<Error, int>(2), Left<Error, int>(Error.New("alternative"))}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb == Left(Error.New("alternative")));
        }
    }
}
