﻿using System;
using Xunit;

namespace LanguageExt.Tests
{
    public class SeqConsTests
    {
        [Fact]
        public void TestEmpty()
        {
            var arr = Seq<int>.Empty;

            var seq = toSeq(arr);

            Assert.True(seq.IsEmpty);
            Assert.True(seq.Tail.IsEmpty);
            Assert.True(seq.Tail.Tail.IsEmpty);

            Assert.Throws<InvalidOperationException>(() => seq.Head);

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
            Assert.Throws<InvalidOperationException>(() => skipped.Head);
        }

        [Fact]
        public void TestOne()
        {
            var arr = 1.Cons();

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
            Assert.Throws<InvalidOperationException>(() => skipped.Head);
        }
        
        static int Sum(Seq<int> seq) =>
            seq.Match(
                ()      => 0,
                x       => x,
                (x, xs) => x + Sum(xs));

        [Fact]
        public void TestMore()
        {
            var arr = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons()))));

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
            var arr = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons()))));

            var seq1 = toSeq(arr);
            var seq2 = seq1.Map(x => x * 2);
            var seq3 = seq1.Select(x => x * 2);
            var seq4 = from x in seq1
                       select x * 2;

            var expected = Seq(2, 4, 6, 8, 10);

            Assert.True(expected == seq2);
            Assert.True(expected == seq3);
            Assert.True(expected == seq4);
        }

        [Fact]
        public void FilterTest()
        {
            var arr = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons()))));

            var seq1 = toSeq(arr);
            var seq2 = seq1.Filter(x => x % 2 == 0);
            var seq3 = seq1.Where(x => x % 2 == 0);
            var seq4 = from x in seq1
                       where x % 2 == 0
                       select x;

            var expected = Seq(2, 4);

            Assert.True(expected == seq2);
            Assert.True(expected == seq3);
            Assert.True(expected == seq4);
        }

        [Fact]
        public void BindTest()
        {
            var seq1 = 10.Cons(100.Cons());
            var seq2 = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons()))));

            var seq3 = seq1.Bind(x => seq2.Map(y => x * y));

            var expected = Seq(10, 20, 30, 40, 50, 100, 200, 300, 400, 500);

            Assert.True(seq3 == expected);
        }

        [Fact]
        public void FoldTest1()
        {
            var seq = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons()))));

            var res1 = seq.Fold(1, (s, x) => s * x);
            var res2 = seq.FoldBack(1, (s, x) => s * x);

            Assert.True(res1 == 120);
            Assert.True(res2 == 120);
        }

        [Fact]
        public void FoldTest2()
        {
            var seq = "a".Cons("b".Cons("c".Cons("d".Cons("e".Cons()))));

            var res1 = seq.Fold("", (s, x) => s + x);
            var res2 = seq.FoldBack("", (s, x) => s + x);

            Assert.True(res1 == "abcde");
            Assert.True(res2 == "edcba");
        }

        [Fact]
        public void Existential()
        {
            var Seq = 'a'.Cons('b'.Cons('c'.Cons('d'.Cons('e'.Cons()))));
            var seq2 = 'a'.Cons('b'.Cons('c'.Cons('_'.Cons('e'.Cons()))));

            var ex1 = Seq.Exists(x => x == 'd');
            var ex2 = seq2.Exists(x => x == 'd');

            var fa1 = Seq.ForAll(Char.IsLetter);
            var fa2 = seq2.ForAll(Char.IsLetter);

            Assert.True(ex1);
            Assert.False(ex2);

            Assert.True(fa1);
            Assert.False(fa2);
        }
    }
}
