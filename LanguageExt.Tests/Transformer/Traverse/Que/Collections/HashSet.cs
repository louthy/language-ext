using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class HashSetQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetQueCrossProduct()
        {
            var ma = HashSet(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);
            mb = toQueue(
                mb.OrderBy(x => x.ToArray()[1])
                    .OrderBy(x => x.ToArray()[0]));

            var mc = Queue(
                HashSet(1, 10),
                HashSet(1, 20),
                HashSet(1, 30),
                HashSet(2, 10),
                HashSet(2, 20),
                HashSet(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(Queue<int>(), Queue<int>(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Que<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(Queue<int>(), Queue<int>());
            var mb = ma.Traverse(identity);
            var mc = Que<HashSet<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
