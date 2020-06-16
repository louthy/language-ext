using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class ArrQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Arr<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrQueCrossProduct()
        {
            var ma = Array(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
                Array(1, 10),
                Array(1, 20),
                Array(1, 30),
                Array(2, 10),
                Array(2, 20),
                Array(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = Array(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void ArrOfEmptiesIsEmpty()
        {
            var ma = Array(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Arr<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
