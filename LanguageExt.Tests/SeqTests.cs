using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;
using static LanguageExt.Seq;

namespace LanguageExt.Tests
{
    public class SeqTests
    {
        [Fact]
        public void ObjectExists()
        {
            var x = "test";

            Assert.True(Seq1(x).Count() == 1);
            Assert.True(Seq1(x).Head() == x);
        }

        [Fact]
        public void ObjectNull()
        {
            string x = null;

            Assert.True(toSeq(x).Count() == 0);
        }

        [Fact]
        public void EnumerableExists()
        {
            var x = new[] { "a", "b", "c" };

            var y = toSeq(x);

            var res = toSeq(x).Tail().AsEnumerable().ToList();


            Assert.True(toSeq(x).Count == 3);
            Assert.True(toSeq(x).Head == "a");
            Assert.True(toSeq(x).Tail.Head == "b");
            Assert.True(toSeq(x).Tail.Tail.Head == "c");
        }

        [Fact]
        public void EnumerableNull()
        {
            string[] x = null;

            Assert.True(toSeq(x).Count == 0);
        }

        [Fact]
        public void TakeTest()
        {
            IEnumerable<int> Numbers()
            {
                for (int i = 0; i < 100; i++)
                {
                    yield return i;
                }
            }

            var seq = Numbers().ToSeq();

            var a = seq.Take(5).Sum();
            Assert.True(a == 10);

            var b = seq.Skip(5).Take(5).Sum();
            Assert.True(b == 35);

        }

        [Fact]
        public void FoldTest()
        {
            var input = Seq(1, 2, 3, 4, 5);
            var output1 = fold(input, "", (s, x) => s + x.ToString());
            Assert.Equal("12345", output1);
        }

        [Fact]
        public void FoldBackTest()
        {
            var input = Seq(1, 2, 3, 4, 5);
            var output1 = foldBack(input, "", (s, x) => s + x.ToString());
            Assert.Equal("54321", output1);
        }

        [Fact]
        public void FoldWhileTest()
        {
            var input = Seq(10, 20, 30, 40, 50);

            var output1 = foldWhile(input, "", (s, x) => s + x.ToString(), x => x < 40);
            Assert.Equal("102030", output1);

            var output2 = foldWhile(input, "", (s, x) => s + x.ToString(), (string s) => s.Length < 6);
            Assert.Equal("102030", output2);

            var output3 = foldWhile(input, 0, (s, x) => s + x, preditem: x => x < 40);
            Assert.Equal(60, output3);

            var output4 = foldWhile(input, 0, (s, x) => s + x, predstate: s => s < 60);
            Assert.Equal(60, output4);
        }

        [Fact]
        public void FoldBackWhileTest()
        {
            var input = Seq(10, 20, 30, 40, 50);

            var output1 = foldBackWhile(input, "", (s, x) => s + x.ToString(), x => x >= 40);
            Assert.Equal("5040", output1);

            var output2 = foldBackWhile(input, "", (s, x) => s + x.ToString(), (string s) => s.Length < 4);
            Assert.Equal("5040", output2);

            var output3 = foldBackWhile(input, 0, (s, x) => s + x, preditem: x => x >= 40);
            Assert.Equal(90, output3);

            var output4 = foldBackWhile(input, 0, (s, x) => s + x, predstate: s => s < 90);
            Assert.Equal(90, output4);
        }

        [Fact]
        public void FoldUntilTest()
        {
            var input = Seq(10, 20, 30, 40, 50);

            var output1 = foldUntil(input, "", (s, x) => s + x.ToString(), x => x >= 40);
            Assert.Equal("102030", output1);

            var output2 = foldUntil(input, "", (s, x) => s + x.ToString(), (string s) => s.Length >= 6);
            Assert.Equal("102030", output2);

            var output3 = foldUntil(input, 0, (s, x) => s + x, preditem: x => x >= 40);
            Assert.Equal(60, output3);

            var output4 = foldUntil(input, 0, (s, x) => s + x, predstate: s => s >= 60);
            Assert.Equal(60, output4);
        }

        [Fact]
        public void FoldBackUntilTest()
        {
            var input = Seq(10, 20, 30, 40, 50);

            var output1 = foldBackUntil(input, "", (s, x) => s + x.ToString(), x => x < 40);
            Assert.Equal("5040", output1);

            var output2 = foldBackUntil(input, "", (s, x) => s + x.ToString(), (string s) => s.Length >= 4);
            Assert.Equal("5040", output2);

            var output3 = foldBackUntil(input, 0, (s, x) => s + x, preditem: x => x < 40);
            Assert.Equal(90, output3);

            var output4 = foldBackUntil(input, 0, (s, x) => s + x, predstate: s => s >= 90);
            Assert.Equal(90, output4);
        }

        [Fact]
        public void EqualityTest_BothNull()
        {
            Seq<int> x = default;
            Seq<int> y = default;

            var eq = x == y;

            Assert.True(eq);
        }

        [Fact]
        public void EqualityTest_LeftNull()
        {
            Seq<int> x = default;
            Seq<int> y = Seq(1, 2, 3);

            var eq = x == y;

            Assert.False(eq);
        }

        [Fact]
        public void EqualityTest_RightNull()
        {
            Seq<int> x = Seq(1, 2, 3);
            Seq<int> y = default;

            var eq = x == y;

            Assert.False(eq);
        }

        [Fact]
        public void EqualityTest()
        {
            Seq<int> x = Seq(1, 2, 3);
            Seq<int> y = Seq(1, 2, 3);

            var eq = x == y;

            Assert.True(eq);
        }

        /// <summary>
        /// Test ensures that lazily evaluated Seq returns the same as strictly evaluated when multiple enumerations occur
        /// </summary>
        [Fact]
        public void GetEnumeratorTest()
        {           
            var res1 = Seq.empty<int>();
            var res2 = Seq.empty<int>();

            IEnumerable<int> a = new List<int> { 1, 2, 3, 4 };

            var s1 = a.ToSeq();
            var s2 = a.ToSeq().Strict();

            foreach (var s in s1)
            {
                if (s == 2)
                {
                    s1.FoldWhile("", (x, y) => "", x => x != 3);
                }
                res1 = res1.Add(s);
            }

            foreach (var s in s2)
            {
                if (s == 2)
                {
                    s2.FoldWhile("", (x, y) => "", x => x != 3);
                }
                res2 = res2.Add(s);
            }

            var eq = res1 == res2;

            Assert.True(eq);
        }

        [Fact]
        public void AddTest()
        {
            var a = Seq1("a");

            var b = a.Add("b");

            var c = a.Add("c");

            Assert.Equal("a", string.Join("|", a));
            Assert.Equal("a|b", string.Join("|", b));
            Assert.Equal("a|c", string.Join("|", c));

        }

        [Fact]
        public void ConsTest()
        {
            var a = Seq1("a");

            var b = "b".Cons(a);

            var c = "c".Cons(a);

            Assert.Equal("a", string.Join("|", a));
            Assert.Equal("b|a", string.Join("|", b));
            Assert.Equal("c|a", string.Join("|", c));

        }

        [Fact]
        public void InitStrictTest()
        {
            var sa = Seq(1, 2, 3, 4, 5);

            var sb = sa.Init;  // [1,2,3,4]
            var sc = sb.Init;  // [1,2,3]
            var sd = sc.Init;  // [1,2]
            var se = sd.Init;  // [1]
            var sf = se.Init;  // []

            Assert.True(sb == Seq(1, 2, 3, 4));
            Assert.True(sc == Seq(1, 2, 3));
            Assert.True(sd == Seq(1, 2));
            Assert.True(se == Seq1(1));
            Assert.True(sf == Empty);
        }

        [Fact]
        public void InitLazyTest()
        {
            var sa = toSeq(Range(1, 5));

            var sb = sa.Init;  // [1,2,3,4]
            var sc = sb.Init;  // [1,2,3]
            var sd = sc.Init;  // [1,2]
            var se = sd.Init;  // [1]
            var sf = se.Init;  // []

            Assert.True(sb == Seq(1, 2, 3, 4));
            Assert.True(sc == Seq(1, 2, 3));
            Assert.True(sd == Seq(1, 2));
            Assert.True(se == Seq1(1));
            Assert.True(sf == Empty);
        }

        [Fact]
        public void InitConcatTest()
        {
            var sa = toSeq(Range(1, 2)) + toSeq(Range(3, 3));

            var sb = sa.Init;  // [1,2,3,4]
            var sc = sb.Init;  // [1,2,3]
            var sd = sc.Init;  // [1,2]
            var se = sd.Init;  // [1]
            var sf = se.Init;  // []

            Assert.True(sb == Seq(1, 2, 3, 4));
            Assert.True(sc == Seq(1, 2, 3));
            Assert.True(sd == Seq(1, 2));
            Assert.True(se == Seq1(1));
            Assert.True(sf == Empty);
        }

        [Fact]
        public void HashTest()
        {
            var s1 = Seq1("test");
            var s2 = Seq1("test");

            Assert.True(s1.GetHashCode() == s2.GetHashCode());
        }
        
        [Fact]
        public void TakeWhileTest()
        {
            var str = "                          <p>The</p>";
            Assert.Equal("                          ",
                String.Join("", str.ToSeq().TakeWhile(ch => ch == ' ')));
        }

        [Fact]
        public void TakeWhileIndex()
        {
            var str = "                          <p>The</p>";
            Assert.Equal("                          ",
                String.Join("", str.ToSeq().TakeWhile((ch, index) => index != 26)));
        }

        [Fact]
        public void TakeWhile_HalfDefaultCapacityTest()
        {
            var str = "1234";
            Assert.Equal("1234", String.Join("", str.ToSeq().TakeWhile(ch => true)));
        }

        [Fact]
        public void TakeWhileIndex_HalfDefaultCapacityTest()
        {
            var str = "1234";
            Assert.Equal("1234", String.Join("", str.ToSeq().TakeWhile((ch, index) => true)));
        }
        
        [Fact]
        public void GeneratingZeroGivesEmptySequence()
        {
            var actual = generate(0, _ => unit);
            Assert.Equal(Seq<Unit>(), actual);
        }

        [Fact]
        public void TakingOneAfterGeneratingZeroGivesEmptySequence()
        {
            var actual = generate(0, _ => unit).Take(1);
            Assert.Equal(Seq<Unit>(), actual);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(4, 5)]
        [InlineData(5, 6)]
        [InlineData(6, 7)]
        [InlineData(7, 8)]
        [InlineData(8, 9)]
        [InlineData(9, 10)]
        [InlineData(10, 20)]
        [InlineData(100, 1000)]
        [InlineData(1000, 10000)]
        public void TakingNAfterGeneratingMoreThanNGivesLengthNSequence(int actualLength, int tryToTake)
        {
            var expect = toSeq(generate(actualLength, identity)).Strict();
            
            var actual = generate(actualLength, identity).Take(tryToTake);
            Assert.Equal(expect, actual);
        }
        
        [Fact]
        public void SeqConcatTest()
        {
            var seq1 = (from x in new[] { 1 }
                        select x).ToSeq();

            var seq2 = (from x in new[] { 2 }
                        select x).ToSeq();


            var seq3 = (from x in new[] { 3 }
                        select x).ToSeq();

            Assert.Equal(
                Seq(1, 2, 3),
                seq1
                   .Concat(seq2)
                   .Concat(seq3));
        }

        [Fact]
        public void CheckItems()
        {
            var xs = Seq<int>();
            Assert.True(xs.Count == 0);
            
            xs = Seq1<int>(0);
            Assert.True(xs.Count == 1);
            Assert.True(xs[0] == 0);
            
            xs = Seq<int>(0, 1);
            Assert.True(xs.Count == 2);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            
            xs = Seq<int>(0, 1, 2);
            Assert.True(xs.Count == 3);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            
            xs = Seq<int>(0, 1, 2, 3);
            Assert.True(xs.Count == 4);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            
            xs = Seq<int>(0, 1, 2, 3, 4);
            Assert.True(xs.Count == 5);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5);
            Assert.True(xs.Count == 6);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6);
            Assert.True(xs.Count == 7);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7);
            Assert.True(xs.Count == 8);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7, 8);
            Assert.True(xs.Count == 9);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            Assert.True(xs[8] == 8);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.True(xs.Count == 10);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            Assert.True(xs[8] == 8);
            Assert.True(xs[9] == 9);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            Assert.True(xs.Count == 11);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            Assert.True(xs[8] == 8);
            Assert.True(xs[9] == 9);
            Assert.True(xs[10] == 10);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            Assert.True(xs.Count == 12);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            Assert.True(xs[8] == 8);
            Assert.True(xs[9] == 9);
            Assert.True(xs[10] == 10);
            Assert.True(xs[11] == 11);
            
            xs = Seq<int>(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            Assert.True(xs.Count == 13);
            Assert.True(xs[0] == 0);
            Assert.True(xs[1] == 1);
            Assert.True(xs[2] == 2);
            Assert.True(xs[3] == 3);
            Assert.True(xs[4] == 4);
            Assert.True(xs[5] == 5);
            Assert.True(xs[6] == 6);
            Assert.True(xs[7] == 7);
            Assert.True(xs[8] == 8);
            Assert.True(xs[9] == 9);
            Assert.True(xs[10] == 10);
            Assert.True(xs[11] == 11);
            Assert.True(xs[12] == 12);
        }

        [Fact]
        void SeqHashCodeRegression()
        {
            // GetHashCode is internally used to compare Seq values => has to be equal irrespective of creation method 

            var originalStrictTail = Seq(2, 3);
            var lazyTailSeq = Set(1, 2, 3).Tail().ToSeq();
            var lazySeqTail = Set(1, 2, 3).ToSeq().Tail;
            var lazyPattern = Set(1, 2, 3).Case is (int _, Seq<int> tail) ? tail : throw new ("invalid");
            
            Assert.Equal(originalStrictTail.GetHashCode(), lazyTailSeq.GetHashCode());
            Assert.Equal(originalStrictTail, lazyTailSeq);
            Assert.Equal(originalStrictTail.GetHashCode(), lazySeqTail.GetHashCode());
            Assert.Equal(originalStrictTail, lazySeqTail);
            Assert.Equal(originalStrictTail.GetHashCode(), lazyPattern.GetHashCode());
            Assert.Equal(originalStrictTail, lazyPattern);
        }
    }
}
