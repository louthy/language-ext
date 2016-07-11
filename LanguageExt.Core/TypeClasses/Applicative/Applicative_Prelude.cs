using LanguageExt.TypeClasses;
using System;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Construct an applicative from an A
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to construct</typeparam>
        /// <typeparam name="A">Type of the applicative value</typeparam>
        /// <param name="a">Applicative value</param>
        /// <returns>Applicative of A</returns>
        public static APPL Pure<APPL, A>(A a)
            where APPL : struct, Applicative<A> =>
                (APPL)default(APPL).Pure(a);

        /// <summary>
        /// Apply applicative functor
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">Type to map from</typeparam>
        /// <typeparam name="B">Type to map to</typeparam>
        /// <param name="x">Applicative functor</param>
        /// <param name="y">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<B> apply<APPL, A, B>(Applicative<Func<A, B>> x, Applicative<A> y) 
            where APPL : struct, Applicative<A> =>
                from a in x
                from b in y
                select a(b);

        /// <summary>
        /// Apply applicative functor
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">Type to map from</typeparam>
        /// <typeparam name="B">Type to map to</typeparam>
        /// <param name="x">Applicative functor</param>
        /// <param name="y">Value to apply</param>
        /// <param name="z">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<C> apply<APPL, A, B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z)
            where APPL : struct, Applicative<A> =>
                from a in x
                from b in y
                from c in z
                select a(b, c);

        /// <summary>
        /// Apply applicative functor
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">Type to map from</typeparam>
        /// <typeparam name="B">Type to map to</typeparam>
        /// <param name="x">Applicative functor</param>
        /// <param name="y">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<Func<B, C>> apply<APPL, A, B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y)
            where APPL : struct, Applicative<A> =>
                from a in x
                from b in y
                select a(b);

        /// <summary>
        /// Apply applicatives in sequence, discarding the result of A
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">First applicative type</typeparam>
        /// <typeparam name="B">Second applicative type</typeparam>
        /// <param name="x">First applicative</param>
        /// <param name="y">Second applicative</param>
        /// <returns>Applicative of B</returns>
        public static Applicative<B> action<APPL, A, B>(Applicative<A> x, Applicative<B> y)
            where APPL : struct, Applicative<A> =>
                from a in x
                from b in y
                select b;

    }
}
