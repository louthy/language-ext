using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class SetQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Set<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetQueCrossProduct()
        {
            var ma = Set(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
                Set(1, 10),
                Set(1, 20),
                Set(1, 30),
                Set(2, 10),
                Set(2, 20),
                Set(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Set(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Set<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void SetOfEmptiesIsEmpty()
        {
            var ma = Set(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Set<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
