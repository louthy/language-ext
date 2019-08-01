using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExt.Tests
{
    public class TypeClassMonoid
    {
        [Fact]
        public void IntMonoid()
        {
            var res = mconcat<TInt, int>(1, 2, 3, 4, 5);

            Assert.True(res == 15);
        }

        [Fact]
        public void ListMonoid()
        {
            var res = mconcat<TLst<int>, Lst<int>>(List(1, 2, 3), List(4, 5));

            Assert.True(res.Sum() == 15 && res.Count == 5);
        }

        [Fact]
        public void StringMonoid()
        {
            var res = mconcat<TString, string>("mary ", "had ", "a ", "little ", "lamb");

            Assert.True(res == "mary had a little lamb");
        }

        [Fact]
        public void MaxIntArrayMonad()
        {
            var xs = new[] { 1, 2, 3, 5, 4 };
            var res = Max<MArray<int>, int[], TInt, int>(xs);

            Assert.True(res == 5);
        }

        [Fact]
        public void MaxStringArrayMonad()
        {
            var xs = new[] { "mary ", "had ", "a ", "little ", "lamb" };

            var res = Max<MArray<string>, string[], TString, string>(xs);

            Assert.True(res == "mary ");
        }

        [Fact]
        public void MaxLstArrayMonad()
        {
            var xs = List("mary ", "had ", "a ", "little ", "lamb");

            var res = Max<MLst<string>, Lst<string>, TString, string>(xs);

            Assert.True(res == "mary ");
        }

        /// <summary>
        /// General purpose maximum value operation
        /// </summary>
        static A Max<MONAD, MA, MONOID, A>(MA ma)
            where MONAD  : struct, Monad<MA, A>
            where MONOID : struct, Ord<A>, Monoid<A> =>
            fold<MONAD, MA, A, A>(ma, default(MONOID).Empty(), Max<MONOID, A>.Inst.Append);
    }
}
