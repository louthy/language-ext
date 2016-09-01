using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Construct an applicative from an A
        /// </summary>
        /// <typeparam name="MA">Type of applicative to construct</typeparam>
        /// <typeparam name="A">Type of the applicative value</typeparam>
        /// <param name="a">Applicative value</param>
        /// <returns>Applicative of A</returns>
        public static MA Pure<MA, A>(A a) where MA : struct, Monad<A> =>
            Return<MA, A>(a);

        /// <summary>
        /// Sequential application (abstract return)
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static Monad<B> apply<A, B>(Monad<Func<A, B>> x, Monad<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential application (abstract return)
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static Monad<C> apply<A, B, C>(Monad<Func<A, B, C>> x, Monad<A> y, Monad<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Sequential application (abstract return)
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static Monad<Func<B, C>> apply<A, B, C>(Monad<Func<A, Func<B, C>>> x, Monad<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential actions (abstract return)
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static Monad<B> action<A, B>(Monad<A> x, Monad<B> y) =>
            from a in x
            from b in y
            select b;


        /// <summary>
        /// Sequential application (specialised return)
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static MB apply<MB, A, B>(Monad<Func<A, B>> x, Monad<A> y)
            where MB : struct, Monad<B> =>
                x.Bind<MB, B>(x, a =>
                y.Bind<MB, B>(y, b =>
                Return<MB, B>(a(b))));

        /// <summary>
        /// Sequential application (specialised return)
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static MC apply<MC, A, B, C>(Monad<Func<A, B, C>> x, Monad<A> y, Monad<B> z)
            where MC : struct, Monad<C> =>
                x.Bind<MC, C>(x, a =>
                y.Bind<MC, C>(y, b =>
                z.Bind<MC, C>(z, c =>
                Return<MC, C>(a(b, c)))));

        /// <summary>
        /// Sequential application (specialised return)
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static MBC apply<MBC, A, B, C>(Monad<Func<A, Func<B, C>>> x, Monad<A> y)
            where MBC : struct, Monad<Func<B, C>> =>
                x.Bind<MBC, Func<B, C>>(x, a =>
                y.Bind<MBC, Func<B, C>>(y, b =>
                Return<MBC, Func<B, C>>(a(b))));

        /// <summary>
        /// Sequential actions (specialised return)
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static MB action<MB, A, B>(Monad<A> x, Monad<B> y)
            where MB : struct, Monad<B> =>
                x.Bind<MB, B>(x, a =>
                y.Bind<MB, B>(y, b =>
                Return<MB, B>(b)));
    }
}
