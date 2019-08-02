using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
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
        public void RecursiveMatchMultiplyTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10, 20, 30, 40, 50);

            Assert.True(Multiply(list0) == 0);
            Assert.True(Multiply(list1) == 10);
            Assert.True(Multiply(list5) == 12000000);
        }

        public int Multiply(IEnumerable<int> list) =>
            list.Match(
                ()      => 0,
                x       => x,
                (x, xs) => x * Multiply(xs));

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
        public void AnotherRecursiveMatchMultiplyTest()
        {
            var list0 = List<int>();
            var list1 = List(10);
            var list5 = List(10, 20, 30, 40, 50);

            Assert.True(AnotherMultiply(list0) == 1);
            Assert.True(AnotherMultiply(list1) == 10);
            Assert.True(AnotherMultiply(list5) == 12000000);
        }

        public int AnotherMultiply(IEnumerable<int> list) =>
            list.Match(
                ()      => 1,
                (x, xs) => x * AnotherMultiply(xs));
    }
}
