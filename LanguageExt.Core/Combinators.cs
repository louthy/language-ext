using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Combinators
    {
        /// <summary>
        /// Identity function, or the Idiot bird (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static A I<A>(A x) => x;

        /// <summary>
        /// The Kestrel (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static A K<A, B>(A x, B y) => x;

        /// <summary>
        /// The Mockingbird (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static Func<A, A> M<A>(Func<A, A> x) =>
            a => x(x(a));

        /// <summary>
        /// The Thrush (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static B T<A, B>(A x, Func<A, B> y) =>
            y(x);

        /// <summary>
        /// The Queer bird (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static C Q<A, B, C>(Func<A, B> x, Func<B, C> y, A z) =>
            y(x(z));

        /// <summary>
        /// The Starling (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static C S<A, B, C>(Func<A, B, C> x, Func<A, B> y, A z) =>
            x(z, y(z));

        /// <summary>
        /// The infamous Y-combinator, or Sage bird (http://dkeenan.com/Lambda/ )
        /// </summary>
        public static B Y<A, B, C>(Func<Func<A, B>, A, B> f, A x) =>
            f(par<Func<Func<A, B>, A, B>, A, B>(Y<A, B, C>, f), x);
    }
}
