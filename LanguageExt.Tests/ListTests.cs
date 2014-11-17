using System.Linq;
using NUnit.Framework;
using LanguageExt;

namespace LanguageExtTests
{
    [TestFixture]
    public class ListTests
    {

        [Test] public void ConsTest1()
        {
            var test = cons(1, cons(2, cons(3, cons(4, list(5)))));

            var array = test.ToArray();

            Assert.IsTrue(array[0] == 1);
            Assert.IsTrue(array[1] == 2);
            Assert.IsTrue(array[2] == 3);
            Assert.IsTrue(array[3] == 4);
            Assert.IsTrue(array[4] == 5);
        }


        
        [Test] public void ListConstruct()
        {
            var test = list(1, 2, 3, 4, 5);

            var array = test.ToArray();

            Assert.IsTrue(array[0] == 1);
            Assert.IsTrue(array[1] == 2);
            Assert.IsTrue(array[2] == 3);
            Assert.IsTrue(array[3] == 4);
            Assert.IsTrue(array[4] == 5);
        }

        [Test] public void MapTest()
        {
            // Generates 10,20,30,40,50
            var input = list(1, 2, 3, 4, 5);
            var output1 = map(input, x => x * 10);

            // Generates 30,40,50
            var output2 = filter(output1, x => x > 20);

            // Generates 120
            var output3 = fold(output2, 0, (x, s) => s + x);

            Assert.IsTrue(output3 == 120);
        }

        [Test] public void MapTestFluent()
        {
            var res = list(1, 2, 3, 4, 5)
                        .map(x => x * 10)
                        .filter(x => x > 20)
                        .fold(0, (x, s) => s + x);

            Assert.IsTrue(res == 120);
        }
    }
}
