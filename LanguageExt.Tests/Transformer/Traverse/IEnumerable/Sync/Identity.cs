using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.IEnumerableT.Sync
{
    public class IdentityIEnumerable
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id(Enumerable.Empty<int>());
            var mb = ma.Traverse(identity);
            var mc = Enumerable.Empty<Identity<int>>();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }

        [Fact]
        public void IdNonEmptyIEnumerableIsIEnumerableId()
        {
            var ma = Id(new[] { 1, 2, 3 }.AsEnumerable());
            var mb = ma.Traverse(identity);
            var mc = new[] { Id(1), Id(2), Id(3) }.AsEnumerable();

            Assert.True(mb.ToSeq() == mc.ToSeq());
        }
    }
}
