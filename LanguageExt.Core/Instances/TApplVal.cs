using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    public struct TApplVal<A> : Applicative<A>
    {
        public A Value;

        TApplVal(A value)
        {
            Value = value;
        }

        public Applicative<B> Action<B>(Applicative<A> x, Applicative<B> y) =>
            y;

        public Applicative<B> Apply<B>(Applicative<Func<A, B>> x, Applicative<A> y) =>
            from f in x
            from a in y
            select f(a);

        public Applicative<Func<B, C>> Apply<B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y) =>
            from f in x
            from a in y
            select f(a);

        public Applicative<C> Apply<B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z) =>
            from f in x
            from a in y
            from b in z
            select f(a, b);

        public Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f) =>
            f(((TApplVal<A>)ma).Value);

        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            new TApplVal<B>(f(((TApplVal<A>)fa).Value));

        public IEnumerable<A> ToSeq(Seq<A> fa)
        {
            yield return ((TApplVal<A>)fa).Value;
        }

        public Applicative<A> Pure(A a) =>
            new TApplVal<A>(a);
    }
}
