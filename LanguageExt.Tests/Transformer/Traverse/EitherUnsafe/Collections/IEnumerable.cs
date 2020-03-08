using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherUnsafeT.Collections
{
    public class IEnumerableEitherUnsafe
    {
        [Fact]
        public void EmptyIEnumerableIsRightEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<EitherUnsafe<Error, int>>();

            var mb = ma.Sequence();

            var mr = mb.Map(b => ma.Count() == b.Count())
                       .IfLeftUnsafe(false);
            
            Assert.True(mr);
        }

        [Fact]
        public void IEnumerableRightsIsRightIEnumerables()
        {
            var ma = new[] {RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), RightUnsafe<Error, int>(3)}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb.Map(b => default(EqEnumerable<int>).Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfLeftUnsafe(false));
        }

        [Fact]
        public void IEnumerableRightAndLeftIsLeftEmpty()
        {
            var ma = new[] {RightUnsafe<Error, int>(1), RightUnsafe<Error, int>(2), LeftUnsafe<Error, int>(Error.New("alternative"))}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb == LeftUnsafe(Error.New("alternative")));
        }
    }
}
