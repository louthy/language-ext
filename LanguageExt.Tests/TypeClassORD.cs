using System.Threading.Tasks;
using Xunit;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class TypeClassORD
    {
        [Fact]
        public void IntOrd()
        {
            Assert.True(lessThan<TInt, int>(10, 20));
            Assert.False(lessThan<TInt, int>(20, 10));
            Assert.False(lessThan<TInt, int>(10, 10));

            Assert.True(greaterThan<TInt, int>(20, 10));
            Assert.False(greaterThan<TInt, int>(10, 20));
            Assert.False(greaterThan<TInt, int>(10, 10));

            Assert.True(lessOrEq<TInt, int>(10, 20));
            Assert.False(lessOrEq<TInt, int>(20, 10));
            Assert.True(lessOrEq<TInt, int>(10, 10));

            Assert.True(greaterOrEq<TInt, int>(20, 10));
            Assert.False(greaterOrEq<TInt, int>(10, 20));
            Assert.True(greaterOrEq<TInt, int>(10, 10));
        }

        [Fact]
        public void DoubleOrd()
        {
            Assert.True(lessThan<TDouble, double>(10, 20));
            Assert.False(lessThan<TDouble, double>(20, 10));
            Assert.False(lessThan<TDouble, double>(10, 10));

            Assert.True(greaterThan<TDouble, double>(20, 10));
            Assert.False(greaterThan<TDouble, double>(10, 20));
            Assert.False(greaterThan<TDouble, double>(10, 10));

            Assert.True(lessOrEq<TDouble, double>(10, 20));
            Assert.False(lessOrEq<TDouble, double>(20, 10));
            Assert.True(lessOrEq<TDouble, double>(10, 10));

            Assert.True(greaterOrEq<TDouble, double>(20, 10));
            Assert.False(greaterOrEq<TDouble, double>(10, 20));
            Assert.True(greaterOrEq<TDouble, double>(10, 10));
        }

        [Fact]
        public void StringOrd()
        {
            Assert.True(lessThan<TString, string>("aaa", "bbb"));
            Assert.False(lessThan<TString, string>("bbb", "aaa"));
            Assert.False(lessThan<TString, string>("aaa", "aaa"));

            Assert.True(greaterThan<TString, string>("bbb", "aaa"));
            Assert.False(greaterThan<TString, string>("aaa", "bbb"));
            Assert.False(greaterThan<TString, string>("aaa", "aaa"));

            Assert.True(lessOrEq<TString, string>("aaa", "bbb"));
            Assert.False(lessOrEq<TString, string>("bbb", "aaa"));
            Assert.True(lessOrEq<TString, string>("aaa", "aaa"));

            Assert.True(greaterOrEq<TString, string>("bbb", "aaa"));
            Assert.False(greaterOrEq<TString, string>("aaa", "bbb"));
            Assert.True(greaterOrEq<TString, string>("aaa", "aaa"));
        }

        [Fact]
        public void MinMax()
        {
            Assert.True(min<OrdInt, int>(1, 2) == 1);
            Assert.True(max<OrdInt, int>(1, 2) == 2);
            Assert.True(min<OrdInt, int>(2, 1) == 1);
            Assert.True(max<OrdInt, int>(2, 1) == 2);
            Assert.True(min<OrdInt, int>(1, 1) == 1);
            Assert.True(max<OrdInt, int>(1, 1) == 1);
        }

        [Fact]
        public void MinMax2()
        {
            Assert.True(min<OrdInt, int>(1, 2, 3, 4) == 1);
            Assert.True(max<OrdInt, int>(1, 2, 3, 4) == 4);
            Assert.True(min<OrdInt, int>(4, 3, 2, 1) == 1);
            Assert.True(max<OrdInt, int>(4, 3, 2, 1) == 4);
            Assert.True(min<OrdInt, int>(1, 1, 1, 1) == 1);
            Assert.True(max<OrdInt, int>(1, 1, 1, 1) == 1);
        }

        private struct OrdDesc<ORD, T> : Ord<T> where ORD : struct, Ord<T>
        {
            public int GetHashCode(T x) => default(ORD).GetHashCode(x);

            public bool Equals(T x, T y) => default(ORD).Equals(x, y);

            public int Compare(T x, T y) => default(ORD).Compare(y, x);

            public Task<int> GetHashCodeAsync(T x) => default(ORD).GetHashCode(x).AsTask();

            public Task<bool> EqualsAsync(T x, T y) => default(ORD).Equals(x, y).AsTask();

            public Task<int> CompareAsync(T x, T y) => default(ORD).Compare(y, x).AsTask();
        }

        [Fact]
        public void OrderBy()
        {
            var items = Prelude.Seq("2", "1", "10");
            
            Assert.Equal(Prelude.Seq("1", "10", "2"), items.OrderBy(Prelude.identity,  default(OrdDefault<string>).ToComparable()));
            Assert.Equal(Prelude.Seq("1", "2", "10"), items.OrderBy(System.Convert.ToInt32,  default(OrdDefault<int>).ToComparable()));
            
            Assert.Equal(Prelude.Seq("2", "10", "1"), items.OrderBy(Prelude.identity,  default(OrdDesc<OrdDefault<string>, string>).ToComparable()));
            Assert.Equal(Prelude.Seq("10", "2", "1"), items.OrderBy(System.Convert.ToInt32,  default(OrdDesc<OrdDefault<int>, int>).ToComparable()));
        }
    }
}
