using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionUnsafeT.Collections
{
    public class IEnumerableOptionUnsafe
    {
        [Fact]
        public void EmptyIEnumerableIsSomeEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<OptionUnsafe<int>>();

            var mb = ma.Sequence();

            var mr = mb.Map(b => ma.Count() == b.Count())
                       .IfNoneUnsafe(false);
            
            Assert.True(mr);
        }

        [Fact]
        public void IEnumerableSomesIsSomeIEnumerables()
        {
            var ma = new[] {SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3)}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb.Map(b => default(EqEnumerable<int>).Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfNoneUnsafe(false));
        }

        [Fact]
        public void IEnumerableSomeAndNoneIsNone()
        {
            var ma = new[] {SomeUnsafe(1), SomeUnsafe(2), None}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
