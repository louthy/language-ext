using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Combinators<A>
    {
        /// <summary>
        /// Identity function, or the Idiot bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<A, A> I = 
            x => x;

        /// <summary>
        /// The Mockingbird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<Func<A, A>, Func<A, A>> M = 
            f => a => f(f(a));
    }

    public static class Combinators<A, B>
    {
        /// <summary>
        /// The Kestrel [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<A, Func<B, A>> K =
            x => _ => x;

        /// <summary>
        /// The Thrush [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<A, Func<Func<A, B>, B>> T =
            x => y => y(x);
    }

    public static class Combinators<A, B, C>
    {
        /// <summary>
        /// The Queer bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<Func<A, B>, Func<Func<B, C>, Func<A, C>>> Q =
            x => y => z => y(x(z));

        /// <summary>
        /// The Starling [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<Func<A, Func<B, C>>, Func<Func<A, B>, Func<A, C>>> S =
            x => y => z => x(z)(y(z));

        /// <summary>
        /// The infamous Y-combinator, or Sage bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static readonly Func<Func<Func<A, B>, A, B>, Func<A, B>> Y =
            f => x => f(Y(f), x);
    }

    public static class Combinators
    {
        /// <summary>
        /// Identity function, or the Idiot bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static A I<A>(A x) => x;

        /// <summary>
        /// The Kestrel [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<B, A> K<A, B>(A x) => _ => x;

        /// <summary>
        /// The Mockingbird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<A, A> M<A>(Func<A, A> x) =>
            a => x(x(a));

        /// <summary>
        /// The Thrush [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, B>, B> T<A, B>(A x) => 
            y => y(x);

        /// <summary>
        /// The Queer bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<B, C>, Func<A, C>> Q<A, B, C>(Func<A, B> x) => 
            y => z => y(x(z));

        /// <summary>
        /// The Starling [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, B>, Func<A, C>> S<A, B, C>(Func<A, B, C> x) => 
            y => z => x(z, y(z));
    }
}
