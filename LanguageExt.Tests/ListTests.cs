using System.Linq;
using NUnit.Framework;
using LanguageExt;

namespace LanguageExtTests
{
    [TestFixture]
    public class ListTests
    {

        [Test] public void ConsTest()
        {
            var test = cons(1, cons(2, cons(3, cons(4, cons(5)))));

            var array = test.ToArray();

            Assert.IsTrue(array[0] == 1);
            Assert.IsTrue(array[1] == 2);
            Assert.IsTrue(array[2] == 3);
            Assert.IsTrue(array[3] == 4);
            Assert.IsTrue(array[4] == 5);
        }
    }
}
