using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Set trait instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MSet<A> :
        Monad<Set<A>, A>,
        Eq<Set<A>>,
        Monoid<Set<A>>
   {
        [Pure]
        public static Set<A> Append(Set<A> x, Set<A> y) =>
            Set.createRange(x.ConcatFast(y));

        [Pure]
        public static MB Bind<MONADB, MB, B>(Set<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
            traverse<MSet<A>, MONADB, Set<A>, MB, A, B>(ma, f);

        [Pure]
        public static Func<Unit, int> Count(Set<A> fa) => _ =>
            fa.Count();

        [Pure]
        public static Set<A> Subtract(Set<A> x, Set<A> y) =>
            Set.createRange(Enumerable.Except(x, y));

        [Pure]
        public static Set<A> Empty() =>
            Set.empty<A>();

        [Pure]
        public static bool Equals(Set<A> x, Set<A> y) =>
            x == y;

        [Pure]
        public static Set<A> Fail(object? err = null) =>
            Empty();

        [Pure]
        public static Func<Unit, S> Fold<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public static Func<Unit, S> FoldBack<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.FoldBack(state, f);

        [Pure]
        public static Set<A> Plus(Set<A> ma, Set<A> mb) =>
            ma + mb;

        [Pure]
        public static Set<A> Return(Func<Unit, A> f) =>
            Set(f(unit));

        [Pure]
        public static Set<A> Zero() =>
            Empty();

        [Pure]
        public static int GetHashCode(Set<A> x) =>
            x.GetHashCode();

        [Pure]
        public static Set<A> Return(A x) =>
            Set(x);

        [Pure]
        public static Set<A> Run(Func<Unit, Set<A>> ma) =>
            ma(unit);

        [Pure]
        public static Set<A> BindReturn(Unit _, Set<A> mb) =>
            mb;

        [Pure]
        public static Set<A> Apply(Func<A, A, A> f, Set<A> fa, Set<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
    }
}
