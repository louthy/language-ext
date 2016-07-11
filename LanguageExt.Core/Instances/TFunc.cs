using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    public struct TFunc<A> : Applicative<Func<A>>
    {
        public Func<A> Value;

        TFunc(Func<A> f)
        {
            Value = f;
        }

        public Applicative<B> Action<B>(Applicative<Func<A>> x, Applicative<B> y) =>
            from a in x
            let _ = a()
            from b in y
            select b;

        public Applicative<B> Apply<B>(Applicative<Func<Func<A>, B>> x, Applicative<Func<A>> y) =>
            from a in x
            from b in y
            select a(b);

        public Applicative<Func<B, C>> Apply<B, C>(Applicative<Func<Func<A>, Func<B, C>>> x, Applicative<Func<A>> y) =>
            from a in x
            from b in y
            select a(b);

        public Applicative<C> Apply<B, C>(Applicative<Func<Func<A>, B, C>> x, Applicative<Func<A>> y, Applicative<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        public Applicative<B> Bind<B>(Applicative<Func<A>> ma, Func<Func<A>, Applicative<B>> f)
        {
            var fn = ((TFunc<A>)ma).Value;
            return f(fn);
        }

        public Functor<B> Map<B>(Functor<Func<A>> fa, Func<Func<A>, B> fab) =>
            Pure<TApplVal<B>, B>(fab(((TFunc<A>)fa).Value));

        public Applicative<Func<A>> Pure(Func<A> a) =>
            new TFunc<A>(a);
    }

    public struct TFunc<A, B> : Applicative<Func<A, B>>
    {
        public Func<A, B> Value;

        public TFunc(Func<A, B> f)
        {
            Value = f;
        }

        public Applicative<C> Action<C>(Applicative<Func<A, B>> x, Applicative<C> y) =>
            from a in x
            from c in y
            select c;

        public Applicative<C> Apply<C>(Applicative<Func<Func<A, B>, C>> x, Applicative<Func<A, B>> y) =>
            from a in x
            from b in y
            select a(b);

        public Applicative<Func<B1, C>> Apply<B1, C>(Applicative<Func<Func<A, B>, Func<B1, C>>> x, Applicative<Func<A, B>> y) =>
            from a in x
            from b in y
            select a(b);

        public Applicative<C> Apply<B1, C>(Applicative<Func<Func<A, B>, B1, C>> x, Applicative<Func<A, B>> y, Applicative<B1> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        public Applicative<B1> Bind<B1>(Applicative<Func<A, B>> fa, Func<Func<A, B>, Applicative<B1>> f) =>
            f(((TFunc<A, B>)fa).Value);

        public Functor<B1> Map<B1>(Functor<Func<A, B>> fa, Func<Func<A, B>, B1> f) =>
            Pure<TApplVal<B1>, B1>(f(((TFunc<A, B>)fa).Value));

        public Applicative<Func<A, B>> Pure(Func<A, B> a) =>
            new TFunc<A, B>(a);
    }
}
