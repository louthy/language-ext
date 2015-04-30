using NUnit.Framework;
using static LanguageExt.Prelude;
using static LanguageExt.Query;

namespace LanguageExtTests
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(list(1, 2, 3, 4, 5));

            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test]
        public void ReduceTest1()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(list(1, 2, 3, 4, 5));
            var output1 = map(input, (i,x) => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test]
        public void ReduceTest2()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(list(1, 2, 3, 4, 5));
            var output1 = map(input, (i, x) => (i + 1) * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test]
        public void MapTestFluent()
        {
            var res = toQuery(list(1, 2, 3, 4, 5))
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            Assert.IsTrue(res == 120);
        }

        [Test]
        public void ReduceTestFluent()
        {
            var res = toQuery(list(1, 2, 3, 4, 5))
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            Assert.IsTrue(res == 120);
        }
    }
}
