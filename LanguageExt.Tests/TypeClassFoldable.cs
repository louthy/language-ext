using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;
using System;

namespace LanguageExtTests
{
    public class TypeClassFoldable
    {
        [Fact]
        public void Tuple3Sum()
        {
            var tup = Tuple(1, 2, 4);

            var total = tup.Sum<TInt, FoldTuple<int>, Tuple<int, int, int>, int>();

            Assert.True(total == 7);
        }

        [Fact]
        public void Tuple3Product()
        {
            var tup = Tuple(2, 4, 8);

            var total = tup.Product<TInt, FoldTuple<int>, Tuple<int, int, int>, int>();

            Assert.True(total == 64);
        }

        [Fact]
        public void Tuple3Contains()
        {
            var tup = Tuple(2, 4, 8);

            bool res = tup.Contains<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(8);

            Assert.True(res);
        }

        [Fact]
        public void Tuple3NotContains()
        {
            var tup = Tuple(2, 4, 8);

            bool res = tup.Contains<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(16);

            Assert.False(res);
        }

        [Fact]
        public void Tuple3Last()
        {
            var tup = Tuple(2, 4, 8);

            var res = tup.Last<FoldTuple<int>, Tuple<int, int, int>, int>();

            Assert.True(res == 8);
        }

        [Fact]
        public void Tuple3First()
        {
            var tup = Tuple(2, 4, 8);

            var res = tup.Head<FoldTuple<int>, Tuple<int, int, int>, int>();

            Assert.True(res == 2);
        }

        [Fact]
        public void Tuple3Seq()
        {
            var tup = Tuple(2, 4, 8);

            var res = tup.ToSeq<FoldTuple<int>, Tuple<int, int, int>, int>().ToArray();

            Assert.True(res.Length == 3);
            Assert.True(res[0] == 2);
            Assert.True(res[1] == 4);
            Assert.True(res[2] == 8);
        }
    }
}
