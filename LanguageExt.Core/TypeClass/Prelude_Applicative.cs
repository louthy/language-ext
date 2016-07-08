using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public static partial class Prelude
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
        /// <param name="f">Applicative functor</param>
        /// <param name="a">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<B> apply<APPL, A, B>(Applicative<Func<A, B>> f, Applicative<A> a) 
            where APPL : struct, Applicative<A> =>
                default(APPL).Apply(f, a);

        /// <summary>
        /// Apply applicative functor
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">Type to map from</typeparam>
        /// <typeparam name="B">Type to map to</typeparam>
        /// <param name="f">Applicative functor</param>
        /// <param name="a">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<C> apply<APPL, A, B, C>(Applicative<Func<A, B, C>> f, Applicative<A> a, Applicative<B> b)
            where APPL : struct, Applicative<A> =>
                default(APPL).Apply(f, a, b);

        /// <summary>
        /// Apply applicative functor
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">Type to map from</typeparam>
        /// <typeparam name="B">Type to map to</typeparam>
        /// <param name="f">Applicative functor</param>
        /// <param name="a">Value to apply</param>
        /// <returns>Mapped applicative</returns>
        public static Applicative<Func<B, C>> apply<APPL, A, B, C>(Applicative<Func<A, Func<B, C>>> f, Applicative<A> a)
            where APPL : struct, Applicative<A> =>
                default(APPL).Apply(f, a);

        /// <summary>
        /// Apply applicatives in sequence, discarding the result of A
        /// </summary>
        /// <typeparam name="APPL">Type of applicative to perform the operation on</typeparam>
        /// <typeparam name="A">First applicative type</typeparam>
        /// <typeparam name="B">Second applicative type</typeparam>
        /// <param name="a">First applicative</param>
        /// <param name="b">Second applicative</param>
        /// <returns>Applicative of B</returns>
        public static Applicative<B> action<APPL, A, B>(Applicative<A> a, Applicative<B> b)
            where APPL : struct, Applicative<A> =>
                default(APPL).Action(a, b);

    }
}
