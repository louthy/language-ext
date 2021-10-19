using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class CombinatorsDynamic
    {
        /// <summary>
        /// Identity function, or the Idiot bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, dynamic> I =
            (dynamic x) =>
                x;

        /// <summary>
        /// The Mockingbird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, dynamic>> M =
            (dynamic x) =>
                (dynamic a) =>
                    x(x(a));

        /// <summary>
        /// The Kestrel [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, dynamic>> K =
            (dynamic x) =>
                (dynamic y) =>
                    x;

        /// <summary>
        /// The Thrush [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, dynamic>> T =
            (dynamic x) =>
                (dynamic y) =>
                    y(x);

        /// <summary>
        /// The Queer bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, Func<dynamic, dynamic>>> Q =
            (dynamic x) =>
                (dynamic y) =>
                    (dynamic z) =>
                        y(x(z));

        /// <summary>
        /// The Starling [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, Func<dynamic, dynamic>>> S =
            (dynamic x) =>
                (dynamic y) =>
                    (dynamic z) =>
                        x(z)(y(z));

        /// <summary>
        /// The infamous Y-combinator, or Sage bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<dynamic, Func<dynamic, dynamic>> Y =
            (dynamic f) =>
                (dynamic x) =>
                    f(Y(f), x);
    }

    public static class Combinators<A>
    {
        /// <summary>
        /// Identity function, or the Idiot bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<A, A> I = 
            (A x) => 
                x;

        /// <summary>
        /// The Mockingbird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, A>, Func<A, A>> M = 
            (Func<A, A> x) =>
                a => 
                    x(x(a));
    }

    public static class Combinators<A, B>
    {
        /// <summary>
        /// The Kestrel [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<A, Func<B, A>> K =
            (A x) =>
                (B y) =>
                    x;

        /// <summary>
        /// The Thrush [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<A, Func<Func<A, B>, B>> T =
            (A x) =>
                (Func<A, B> y) =>
                    y(x);
    }

    public static class Combinators<A, B, C>
    {
        /// <summary>
        /// The Queer bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, B>, Func<Func<B, C>, Func<A, C>>> Q = 
            (Func<A, B> x) => 
                (Func<B, C> y) => 
                    (A z) =>
                        y(x(z));

        /// <summary>
        /// The Starling [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, Func<B, C>>, Func<Func<A, B>, Func<A, C>>> S = 
            (Func<A, Func<B, C>> x) => 
                (Func<A, B> y) => 
                    (A z) =>
                        x(z)(y(z));

        /// <summary>
        /// The infamous Y-combinator, or Sage bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<Func<A, B>, A, B>, Func<A, B>> Y = 
            (Func<Func<A, B>, A, B> f) => 
                (A x) =>
                    f(Y(f), x);
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
        public static Func<B, A> K<A, B>(A x) => (B y) => x;

        /// <summary>
        /// The Mockingbird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<A, A> M<A>(Func<A, A> x) =>
            a => x(x(a));

        /// <summary>
        /// The Thrush [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, B>, B> T<A, B>(A x) => (Func<A, B> y) =>
            y(x);

        /// <summary>
        /// The Queer bird [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<B, C>, Func<A, C>> Q<A, B, C>(Func<A, B> x) => (Func<B, C> y) => (A z) =>
             y(x(z));

        /// <summary>
        /// The Starling [dkeenan.com/Lambda/](http://dkeenan.com/Lambda/)
        /// </summary>
        public static Func<Func<A, B>, Func<A, C>> S<A, B, C>(Func<A, B, C> x) => (Func<A, B> y) => (A z) =>
            x(z, y(z));
    }
}
