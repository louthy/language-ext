using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionT.Collections
{
    public class IEnumerableOption
    {
        [Fact]
        public void EmptyIEnumerableIsSomeEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<Option<int>>();

            var mb = ma.Sequence();

            var mr = mb.Map(b => ma.Count() == b.Count())
                       .IfNone(false);
            
            Assert.True(mr);
        }

        [Fact]
        public void IEnumerableSomesIsSomeIEnumerables()
        {
            var ma = new[] {Some(1), Some(2), Some(3)}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb.Map(b => default(EqEnumerable<int>).Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfNone(false));
        }

        [Fact]
        public void IEnumerableSomeAndNoneIsNone()
        {
            var ma = new[] {Some(1), Some(2), None}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb == None);
        }
    }
}
