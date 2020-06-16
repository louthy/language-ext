using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Collections
{
    public class LstQue
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<Que<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = Que<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstQueCrossProduct()
        {
            var ma = List(Queue(1, 2), Queue(10, 20, 30));

            var mb = ma.Traverse(identity);

            var mc = Queue(
                List(1, 10),
                List(1, 20),
                List(1, 30),
                List(2, 10),
                List(2, 20),
                List(2, 30));

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = List(Queue<int>(), Queue<int>(1, 2, 3));

            var mb = ma.Traverse(identity);

            var mc = Que<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List(Queue<int>(), Queue<int>());

            var mb = ma.Traverse(identity);

            var mc = Que<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
