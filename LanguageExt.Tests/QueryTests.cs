using Xunit;
using static LanguageExt.Prelude;
using static LanguageExt.Query;

namespace LanguageExt.Tests
{
    
    public class QueryTests
    {
        [Fact]
        public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(new[] { 1, 2, 3, 4, 5 });

            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            Assert.True(output3 == 120);
        }

        [Fact]
        public void ReduceTest1()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(new[] { 1, 2, 3, 4, 5 });
            var output1 = map(input, (i,x) => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.True(output3 == 120);
        }

        [Fact]
        public void ReduceTest2()
        {
            // Generates 10,20,30,40,50
            var input = toQuery(new[] { 1, 2, 3, 4, 5 });
            var output1 = map(input, (i, x) => (i + 1) * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = reduce(output2, (x, s) => s + x);

            Assert.True(output3 == 120);
        }

        [Fact]
        public void MapTestFluent()
        {
            var res = toQuery(new[] { 1, 2, 3, 4, 5 })
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Fold(0, (x, s) => s + x);

            Assert.True(res == 120);
        }

        [Fact]
        public void ReduceTestFluent()
        {
            var res = toQuery(new[] { 1, 2, 3, 4, 5 })
                        .Map(x => x * 10)
                        .Filter(x => x > 20)
                        .Reduce((x, s) => s + x);

            Assert.True(res == 120);
        }
    }
}
