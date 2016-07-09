using System;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass.Prelude;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Applicative functor type-class
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public interface AP<A> : Functor<A>
    {
        /// <summary>
        /// Applicative construction
        /// 
        ///     a -> f a
        /// </summary>
        AP<A> Pure(A a);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        AP<B> Apply<B>(AP<Func<A, B>> x, AP<A> y);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        AP<C> Apply<B, C>(AP<Func<A, B, C>> x, AP<A> y, AP<B> z);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        AP<Func<B,C>> Apply<B, C>(AP<Func<A, Func<B, C>>> x, AP<A> y);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        AP<B> Action<B>(AP<A> x, AP<B> y);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        AP<B> Bind<B>(AP<A> ma, Func<A, AP<B>> f);
    }

    public struct TApplVal<A> : AP<A>
    {
        public A Value;

        TApplVal(A value)
        {
            Value = value;
        }

        public AP<B> Action<B>(AP<A> x, AP<B> y) =>
            y;

        public AP<B> Apply<B>(AP<Func<A, B>> x, AP<A> y) =>
            from f in x
            from a in y
            select f(a);

        public AP<Func<B, C>> Apply<B, C>(AP<Func<A, Func<B, C>>> x, AP<A> y) =>
            from f in x
            from a in y
            select f(a);

        public AP<C> Apply<B, C>(AP<Func<A, B, C>> x, AP<A> y, AP<B> z) =>
            from f in x
            from a in y
            from b in z
            select f(a, b);

        public AP<B> Bind<B>(AP<A> ma, Func<A, AP<B>> f) =>
            f(((TApplVal<A>)ma).Value);

        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            new TApplVal<B>(f(((TApplVal<A>)fa).Value));

        public Unit Iter(Iterable<A> fa, Action<A> f)
        {
            f(((TApplVal<A>)fa).Value);
            return unit;
        }

        public AP<A> Pure(A a) =>
            new TApplVal<A>(a);
    }

    public struct TFunc<A> : AP<Func<A>>
    {
        public Func<A> Value;

        TFunc(Func<A> f)
        {
            Value = f;
        }

        public AP<B> Action<B>(AP<Func<A>> x, AP<B> y) =>
            from a in x
            let _ = a()
            from b in y
            select b;

        public AP<B> Apply<B>(AP<Func<Func<A>, B>> x, AP<Func<A>> y) =>
            from a in x
            from b in y
            select a(b);

        public AP<Func<B, C>> Apply<B, C>(AP<Func<Func<A>, Func<B, C>>> x, AP<Func<A>> y) =>
            from a in x
            from b in y
            select a(b);

        public AP<C> Apply<B, C>(AP<Func<Func<A>, B, C>> x, AP<Func<A>> y, AP<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        public AP<B> Bind<B>(AP<Func<A>> ma, Func<Func<A>, AP<B>> f)
        {
            var fn = ((TFunc<A>)ma).Value;
            return f(fn);
        }

        public Functor<B> Map<B>(Functor<Func<A>> fa, Func<Func<A>, B> fab) =>
            Pure<TApplVal<B>,B>(fab(((TFunc<A>)fa).Value));

        public Unit Iter(Iterable<Func<A>> fa, Action<Func<A>> f)
        {
            f(((TFunc<A>)fa).Value);
            return unit;
        }

        public AP<Func<A>> Pure(Func<A> a) =>
            new TFunc<A>(a);
    }

    public struct TFunc<A, B> : AP<Func<A, B>>
    {
        public Func<A, B> Value;

        public TFunc(Func<A,B> f)
        {
            Value = f;
        }

        public AP<C> Action<C>(AP<Func<A, B>> x, AP<C> y) =>
            from a in x
            from c in y
            select c;

        public AP<C> Apply<C>(AP<Func<Func<A, B>, C>> x, AP<Func<A, B>> y) =>
            from a in x
            from b in y
            select a(b);

        public AP<Func<B1, C>> Apply<B1, C>(AP<Func<Func<A, B>, Func<B1, C>>> x, AP<Func<A, B>> y) =>
            from a in x
            from b in y
            select a(b);

        public AP<C> Apply<B1, C>(AP<Func<Func<A, B>, B1, C>> x, AP<Func<A, B>> y, AP<B1> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        public AP<B1> Bind<B1>(AP<Func<A, B>> fa, Func<Func<A, B>, AP<B1>> f) =>
            f(((TFunc<A, B>)fa).Value);

        public Functor<B1> Map<B1>(Functor<Func<A, B>> fa, Func<Func<A, B>, B1> f) =>
            Pure<TApplVal<B1>, B1>(f(((TFunc<A, B>)fa).Value));

        public Unit Iter(Iterable<Func<A,B>> fa, Action<Func<A,B>> f)
        {
            f(((TFunc<A,B>)fa).Value);
            return unit;
        }

        public AP<Func<A, B>> Pure(Func<A, B> a) =>
            new TFunc<A, B>(a);
    }
}
