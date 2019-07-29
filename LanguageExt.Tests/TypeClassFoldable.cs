using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;
using System;

namespace LanguageExt.Tests
{
    public class TypeClassFoldable
    {
        [Fact]
        public void Tuple3Sum()
        {
            var tup = Tuple(1, 2, 4);

            var total = sum<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(tup);

            Assert.True(total == 7);
        }

        [Fact]
        public void Tuple3Product()
        {
            var tup = Tuple(2, 4, 8);

            var total = product<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(tup);

            Assert.True(total == 64);
        }

        [Fact]
        public void Tuple3Contains()
        {
            var tup = Tuple(2, 4, 8);

            bool res = contains<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(tup, 8);

            Assert.True(res);
        }

        [Fact]
        public void Tuple3NotContains()
        {
            var tup = Tuple(2, 4, 8);

            bool res = contains<TInt, FoldTuple<int>, Tuple<int, int, int>, int>(tup, 16);

            Assert.False(res);
        }

        [Fact]
        public void Tuple3Last()
        {
            var tup = Tuple(2, 4, 8);

            var res = last<FoldTuple<int>, Tuple<int, int, int>, int>(tup);

            Assert.True(res == 8);
        }

        [Fact]
        public void Tuple3First()
        {
            var tup = Tuple(2, 4, 8);

            var res = head<FoldTuple<int>, Tuple<int, int, int>, int>(tup);

            Assert.True(res == 2);
        }

        [Fact]
        public void Tuple3Seq()
        {
            var tup = Tuple(2, 4, 8);

            var res = toSeq<FoldTuple<int>, Tuple<int, int, int>, int>(tup).ToArray();

            Assert.True(res.Length == 3);
            Assert.True(res[0] == 2);
            Assert.True(res[1] == 4);
            Assert.True(res[2] == 8);
        }
    }
}
