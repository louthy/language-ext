using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Sequential application (abstract return)
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static Monad<B> Apply<A, B>(this Monad<Func<A, B>> x, Monad<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential application (abstract return)
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static Monad<C> Apply<A, B, C>(this Monad<Func<A, B, C>> x, Monad<A> y, Monad<B> z) =>
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
        public static Monad<Func<B, C>> Apply<A, B, C>(this Monad<Func<A, Func<B, C>>> x, Monad<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential actions (abstract return)
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static Monad<B> Action<A, B>(this Monad<A> x, Monad<B> y) =>
            from a in x
            from b in y
            select b;


        /// <summary>
        /// Sequential application (specialised return)
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static MB Apply<MB, A, B>(this Monad<Func<A, B>> x, Monad<A> y) 
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
        public static MC Apply<MC, A, B, C>(this Monad<Func<A, B, C>> x, Monad<A> y, Monad<B> z) 
            where MC : struct, Monad<C> =>
                x.Bind<MC, C>(x, a => 
                y.Bind<MC, C>(y, b => 
                z.Bind<MC, C>(z, c => 
                Return<MC, C>(a(b,c)))));

        /// <summary>
        /// Sequential application (specialised return)
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static MBC Apply<MBC, A, B, C>(this Monad<Func<A, Func<B, C>>> x, Monad<A> y)
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
        public static MB Action<MB, A, B>(this Monad<A> x, Monad<B> y)
            where MB : struct, Monad<B> =>
                x.Bind<MB, B>(x, a => 
                y.Bind<MB, B>(y, b => 
                Return<MB, B>(b)));
    }
}
