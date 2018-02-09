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
                for (int i = 0; i < Int32.MaxValue; i++)
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
    }
}
