using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;
using Array = System.Array;

namespace LanguageExt.Tests
{
    public class SeqEnumerableTests
    {
        readonly IEnumerable<int> EmptyList = Array.Empty<int>();
        readonly IEnumerable<int> OneItem = [1];
        readonly IEnumerable<int> FiveItems = [1, 2, 3, 4, 5];
        readonly IEnumerable<int> TenHundred = [10, 100];
        readonly IEnumerable<int> DoubleFiveItems = [2, 4, 6, 8, 10];
        readonly IEnumerable<int> EvenItems = [2, 4];
        readonly IEnumerable<int> BoundItems = [10, 20, 30, 40, 50, 100, 200, 300, 400, 500];
        readonly IEnumerable<char> abcdeChars = ['a', 'b', 'c', 'd', 'e'];
        readonly IEnumerable<char> abc_eChars = ['a', 'b', 'c', '_', 'e'];
        readonly IEnumerable<char> edcbaChars = ['e', 'd', 'c', 'b', 'a'];
        readonly IEnumerable<string> abcdeStrs = ["a", "b", "c", "d", "e"];
        readonly IEnumerable<string> edcbaStrs = ["e", "d", "c", "b", "a"];

        [Fact]
        public void TestEmpty()
        {
            var arr = EmptyList;

            var seq = toSeq(arr);

            Assert.True(seq.IsEmpty);
            Assert.True(seq.Tail.IsEmpty);
            Assert.True(seq.Tail.Tail.IsEmpty);
            Assert.True(seq.Head.IsNone);
            Assert.True(seq.Count == 0);
            Assert.True(seq.Count() == 0);

            var res1 = seq.Match(
                ()      => true,
                (x, xs) => false);

            var res2 = seq.Match(
                ()      => true,
                x       => false,
                (x, xs) => false);

            Assert.True(res1);
            Assert.True(res2);

            var skipped = seq.Skip(1);
            Assert.True(skipped.IsEmpty);
            Assert.True(skipped.Count == 0);
            Assert.True(skipped.Count() == 0);
            Assert.True(skipped.Head.IsNone);
        }

        [Fact]
        public void TestOne()
        {
            var arr = OneItem;

            var seq = toSeq(arr);

            Assert.True(seq.Head == 1);
            Assert.True(seq.Tail.IsEmpty);
            Assert.True(seq.Tail.Tail.IsEmpty);

            Assert.True(seq.Count == 1);
            Assert.True(seq.Count() == 1);

            var res1 = seq.Match(
                ()      => false,
                (x, xs) => x == 1 && xs.IsEmpty);

            var res2 = seq.Match(
                ()      => false,
                x       => x == 1,
                (x, xs) => false);

            Assert.True(res1);
            Assert.True(res2);

            var skipped = seq.Skip(1);
            Assert.True(skipped.IsEmpty);
            Assert.True(skipped.Count == 0);
            Assert.True(skipped.Count() == 0);
            Assert.True(skipped.Head.IsNone);
        }
        
        static int Sum(Seq<int> seq) =>
            seq.Match(
                ()      => 0,
                x       => x,
                (x, xs) => x + Sum(xs));

        [Fact]
        public void TestMore()
        {
            var arr = FiveItems;

            var seq = toSeq(arr);

            Assert.True(seq.Head == 1);
            Assert.True(seq.Tail.Head == 2);
            Assert.True(seq.Tail.Tail.Head == 3);
            Assert.True(seq.Tail.Tail.Tail.Head == 4);
            Assert.True(seq.Tail.Tail.Tail.Tail.Head == 5);

            Assert.True(seq.Tail.Tail.Tail.Tail.Tail.IsEmpty);

            Assert.True(seq.Count == 5);
            Assert.True(seq.Count() == 5);

            Assert.True(seq.Tail.Count == 4);
            Assert.True(seq.Tail.Count() == 4);

            Assert.True(seq.Tail.Tail.Count == 3);
            Assert.True(seq.Tail.Tail.Count() == 3);

            Assert.True(seq.Tail.Tail.Tail.Count == 2);
            Assert.True(seq.Tail.Tail.Tail.Count() == 2);

            Assert.True(seq.Tail.Tail.Tail.Tail.Count == 1);
            Assert.True(seq.Tail.Tail.Tail.Tail.Count() == 1);

            var res = Sum(seq);

            Assert.True(res == 15);

            var skipped1 = seq.Skip(1);
            Assert.True(skipped1.Head == 2);
            Assert.True(skipped1.Count == 4);
            Assert.True(skipped1.Count() == 4);

            var skipped2 = seq.Skip(2);
            Assert.True(skipped2.Head == 3);
            Assert.True(skipped2.Count == 3);
            Assert.True(skipped2.Count() == 3);

            var skipped3 = seq.Skip(3);
            Assert.True(skipped3.Head == 4);
            Assert.True(skipped3.Count == 2);
            Assert.True(skipped3.Count() == 2);

            var skipped4 = seq.Skip(4);
            Assert.True(skipped4.Head == 5);
            Assert.True(skipped4.Count == 1);
            Assert.True(skipped4.Count() == 1);

            var skipped5 = seq.Skip(5);
            Assert.True(skipped5.IsEmpty);
            Assert.True(skipped5.Count == 0);
            Assert.True(skipped5.Count() == 0);
        }

        [Fact]
        public void MapTest()
        {
            var arr = FiveItems;

            var seq1 = toSeq(arr);
            var seq2 = seq1.Map(x => x * 2);
            var seq3 = seq1.Select(x => x * 2);
            var seq4 = from x in seq1
                       select x * 2;

            var expected = toSeq(DoubleFiveItems);

            Assert.True(expected == seq2);
            Assert.True(expected == seq3);
            Assert.True(expected == seq4);
        }

        [Fact]
        public void FilterTest()
        {
            var arr = FiveItems;

            var seq1 = toSeq(arr);
            var seq2 = seq1.Filter(x => x % 2 == 0);
            var seq3 = seq1.Where(x => x % 2 == 0);
            var seq4 = from x in seq1
                       where x % 2 == 0
                       select x;

            var expected = toSeq(EvenItems);

            Assert.True(expected == seq2);
            Assert.True(expected == seq3);
            Assert.True(expected == seq4);
        }

        [Fact]
        public void BindTest()
        {
            var seq1 = toSeq(TenHundred);
            var seq2 = toSeq(FiveItems);

            var seq3 = seq1.Bind(x => seq2.Map(y => x * y));

            var expected = toSeq(BoundItems);

            Assert.True(seq3 == expected);
        }

        [Fact]
        public void FoldTest1()
        {
            var seq = toSeq(FiveItems);

            var res1 = seq.Fold((s, x) => s     * x, 1);
            var res2 = seq.FoldBack((s, x) => s * x, 1);

            Assert.True(res1 == 120);
            Assert.True(res2 == 120);
        }

        [Fact]
        public void FoldTest2()
        {
            var seq = toSeq(abcdeStrs);

            var res1 = seq.Fold((s, x) => s     + x, "");
            var res2 = seq.FoldBack((s, x) => s + x, "");

            Assert.True(res1 == "abcde");
            Assert.True(res2 == "edcba");
        }

        [Fact]
        public void Existential()
        {
            var Seq  = toSeq(abcdeChars);
            var seq2 = toSeq(abc_eChars);

            var ex1 = Seq.Exists(x => x == 'd');
            var ex2 = seq2.Exists(x => x == 'd');

            var fa1 = Seq.ForAll(Char.IsLetter);
            var fa2 = seq2.ForAll(Char.IsLetter);

            Assert.True(ex1);
            Assert.False(ex2);

            Assert.True(fa1);
            Assert.False(fa2);
        }

        [Fact]
        public void TestQueryableCount() =>
            Assert.True(Seq(1, 2, 3).AsQueryable().Count() == 3);
    }
}
