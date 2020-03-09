using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.ArrT.Collections
{
    public class LstArr
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            Lst<Arr<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Arr<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstArrCrossProduct()
        {
            var ma = List(Array(1, 2), Array(10, 20, 30));

            var mb = ma.Sequence();

            var mc = Array(
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
            var ma = List(Array<int>(), Array<int>(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Arr<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }

        [Fact]
        public void LstOfEmptiesIsEmpty()
        {
            var ma = List(Array<int>(), Array<int>());

            var mb = ma.Sequence();

            var mc = Arr<Lst<int>>.Empty;

            Assert.True(mb == mc);
        }
    }
}
