using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class ListMatchingTests
    {
        [Test]
        public void RecursiveMatchSumTest()
        {
            var list0 = list<int>();
            var list1 = list(10);
            var list5 = list(10,20,30,40,50);

            Assert.IsTrue(Sum(list0) == 0);
            Assert.IsTrue(Sum(list1) == 10);
            Assert.IsTrue(Sum(list5) == 150);
        }

        [Test]
        public void RecursiveMatchProductTest()
        {
            var list0 = list<int>();
            var list1 = list(10);
            var list5 = list(10, 20, 30, 40, 50);

            Assert.IsTrue(Product(list0) == 0);
            Assert.IsTrue(Product(list1) == 10);
            Assert.IsTrue(Product(list5) == 12000000);
        }

        public int Sum(IEnumerable<int> list) =>
            match( list,
                   ()      => 0,
                   x       => x,
                   (x, xs) => x + Sum(xs) );

        public int Product(IEnumerable<int> list) =>
            list.Match(
                () => 0,
                x => x,
                (x, xs) => x * Product(xs));

    }
}
