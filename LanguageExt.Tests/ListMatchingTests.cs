using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class ListMatchingTests
    {
        [Fact]
        public void RecursiveMatchSumTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10,20,30,40,50);

            Assert.True(Sum(list0) == 0);
            Assert.True(Sum(list1) == 10);
            Assert.True(Sum(list5) == 150);
        }

        public int Sum(IEnumerable<int> list) =>
            match(list,
                   ()      => 0,
                   x       => x,
                   (x, xs) => x + Sum(xs));

        [Fact]
        public void RecursiveMatchProductTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10, 20, 30, 40, 50);

            Assert.True(Product(list0) == 0);
            Assert.True(Product(list1) == 10);
            Assert.True(Product(list5) == 12000000);
        }

        public int Product(IEnumerable<int> list) =>
            list.Match(
                ()      => 0,
                x       => x,
                (x, xs) => x * Product(xs));

        [Fact]
        public void AnotherRecursiveMatchSumTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10, 20, 30, 40, 50);

            Assert.True(AnotherSum(list0) == 0);
            Assert.True(AnotherSum(list1) == 10);
            Assert.True(AnotherSum(list5) == 150);
        }

        public int AnotherSum(IEnumerable<int> list) =>
            match(list,
                ()      => 0,
                (x, xs) => x + AnotherSum(xs));

        [Fact]
        public void AnotherRecursiveMatchProductTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10, 20, 30, 40, 50);

            Assert.True(AnotherProduct(list0) == 1);
            Assert.True(AnotherProduct(list1) == 10);
            Assert.True(AnotherProduct(list5) == 12000000);
        }

        public int AnotherProduct(IEnumerable<int> list) =>
            list.Match(
                ()      => 1,
                (x, xs) => x * AnotherProduct(xs));

        [Fact]
        public void Match6Fluent()
        {
            IEnumerable<int> listN = null;
            var list0 = List<int>();
            var list1 = List(10);
            var list2 = List(10, 20);
            var list3 = List(10, 20, 30);
            var list4 = List(10, 20, 30, 40);
            var list5 = List(10, 20, 30, 40, 50);
            var list6 = List(10, 20, 30, 40, 50, 60);
            var list100 = Range(1, 100);

            var matcher = fun( (IEnumerable<int> lst) =>
               lst.Match(
                   () => 0,
                   a => 1,
                   (a, b) => 2,
                   (a, b, c) => 3,
                   (a, b, c, d) => 4,
                   (a, b, c, d, e) => 5,
                   (a, b, c, d, e, f) => 6,
                   (a, b, c, d, e, f, xs) => xs.Count() + 6
               ) );

            Assert.True(matcher(listN) == 0);
            Assert.True(matcher(list0) == 0);
            Assert.True(matcher(list1) == 1);
            Assert.True(matcher(list2) == 2);
            Assert.True(matcher(list3) == 3);
            Assert.True(matcher(list4) == 4);
            Assert.True(matcher(list5) == 5);
            Assert.True(matcher(list6) == 6);
            Assert.True(matcher(list100) == 100);
        }

        [Fact]
        public void Match6Func()
        {
            IEnumerable<int> listN = null;
            var list0 = List<int>();
            var list1 = List(10);
            var list2 = List(10, 20);
            var list3 = List(10, 20, 30);
            var list4 = List(10, 20, 30, 40);
            var list5 = List(10, 20, 30, 40, 50);
            var list6 = List(10, 20, 30, 40, 50, 60);
            var list100 = Range(1, 100);

            var matcher = fun((IEnumerable<int> lst) =>
              match(
                  lst,
                  () => 0,
                  a => 1,
                  (a, b) => 2,
                  (a, b, c) => 3,
                  (a, b, c, d) => 4,
                  (a, b, c, d, e) => 5,
                  (a, b, c, d, e, f) => 6,
                  (a, b, c, d, e, f, xs) => xs.Count() + 6
              ));

            Assert.True(matcher(listN) == 0);
            Assert.True(matcher(list0) == 0);
            Assert.True(matcher(list1) == 1);
            Assert.True(matcher(list2) == 2);
            Assert.True(matcher(list3) == 3);
            Assert.True(matcher(list4) == 4);
            Assert.True(matcher(list5) == 5);
            Assert.True(matcher(list6) == 6);
            Assert.True(matcher(list100) == 100);
        }
    }
}
