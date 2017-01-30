using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append an extra item to the tuple
        /// </summary>
        [Pure]
        public static (A, B) add<A, B>(ValueTuple<A> self, B second) =>
            (self.Item1, second);

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static ValueTuple<A> append<SemiA, SemiB, A, B>(ValueTuple<A> a, ValueTuple<A> b)
            where SemiA : struct, Semigroup<A> =>
            VTuple(default(SemiA).Append(a.Item1, b.Item1));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static ValueTuple<A> concat<MonoidA, MonoidB, A, B>(ValueTuple<A> a, ValueTuple<A> b)
            where MonoidA : struct, Monoid<A>
            where MonoidB : struct, Monoid<B> =>
            VTuple(mconcat<MonoidA, A>(a.Item1, b.Item1));

        /// <summary>
        /// Take the first item
        /// </summary>
        [Pure]
        public static A head<A>(ValueTuple<A> self) =>
            self.Item1;

        /// <summary>
        /// Take the last item
        /// </summary>
        [Pure]
        public static A last<A>(ValueTuple<A> self) =>
            self.Item1;

        /// <summary>
        /// Sum of the items
        /// </summary>
        [Pure]
        public static A sum<NUM, A>(ValueTuple<A> self)
            where NUM : struct, Num<A> =>
            self.Item1;

        /// <summary>
        /// Product of the items
        /// </summary>
        [Pure]
        public static A product<NUM, A>(ValueTuple<A> self)
            where NUM : struct, Num<A> =>
            self.Item1;

        /// <summary>
        /// One of the items matches the value passed
        /// </summary>
        [Pure]
        public static bool contains<EQ, A>(ValueTuple<A> self, A value)
            where EQ : struct, Eq<A> =>
            default(EQ).Equals(self.Item1, value);

        /// <summary>
        /// Map to R
        /// </summary>
        [Pure]
        public static ValueTuple<R> map<A, R>(ValueTuple<A> self, Func<A, R> map) =>
            VTuple(map(self.Item1));

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<A>(ValueTuple<A> self, Action<A> func)
        {
            func(self.Item1);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<A, S>(ValueTuple<A> self, S state, Func<S, A, S> fold) =>
            fold(state, self.Item1);
    }
}