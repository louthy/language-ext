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

            Assert.True(Seq(x).Count() == 0);
        }

        [Fact]
        public void EnumerableExists()
        {
            var x = new[] { "a", "b", "c" };

            var y = Seq(x);

            var res = Seq(x).Tail().AsEnumerable().ToList();


            Assert.True(Seq(x).Count() == 3);
            Assert.True(Seq(x).Head() == "a");
            Assert.True(Seq(x).Tail().Head() == "b");
            Assert.True(Seq(x).Tail().Tail().Head() == "c");
        }

        [Fact]
        public void EnumerableNull()
        {
            string[] x = null;

            Assert.True(Seq(x).Count() == 0);
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
            var sa = Seq(Range(1, 5));

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
            var sa = Seq(Range(1, 2)) + Seq(Range(3, 3));

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
    }
}
