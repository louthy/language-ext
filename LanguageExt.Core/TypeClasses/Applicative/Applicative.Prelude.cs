using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
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
        public static MA Pure<MONAD, MA, A>(A x, params A[] xs) where MONAD : struct, Monad<MA, A> =>
            default(MONAD).Return(x);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static MB apply<MONADAB, MONADA, MONADB, MAB, MA, MB, A, B>(MAB x, MA y)
            where MONADAB : struct, Monad<MAB, Func<A, B>>
            where MONADA  : struct, Monad<MA, A>
            where MONADB  : struct, Monad<MB, B> =>
            default(MONADAB).Bind<MONADB, MB, B>(x, a =>
            default(MONADA ).Bind<MONADB, MB, B>(y, b =>
            default(MONADB ).Return(a(b))));

        /// <summary>
        /// Sequential application 
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static MC apply<MONADABC, MONADA, MONADB, MONADC, MABC, MA, MB, MC, A, B, C>(MABC x, MA y, MB z)
            where MONADABC : struct, Monad<MABC, Func<A, B, C>>
            where MONADA   : struct, Monad<MA, A>
            where MONADB   : struct, Monad<MB, B>
            where MONADC   : struct, Monad<MC, C> =>
            default(MONADABC).Bind<MONADC, MC, C>(x, a =>
            default(MONADA  ).Bind<MONADC, MC, C>(y, b =>
            default(MONADB  ).Bind<MONADC, MC, C>(z, c =>
            default(MONADC  ).Return(a(b, c)))));

        /// <summary>
        /// Sequential application 
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        public static MBC apply<MONADABC, MONADA, MONADBC, MABC, MA, MBC, A, B, C>(MABC x, MA y)
            where MONADABC : struct, Monad<MABC, Func<A, B, C>>
            where MONADA   : struct, Monad<MA, A>
            where MONADBC  : struct, Monad<MBC, Func<B, C>> =>
            default(MONADABC).Bind<MONADBC, MBC, Func<B,C>>(x, a =>
            default(MONADA  ).Bind<MONADBC, MBC, Func<B, C>>(y, b =>
            default(MONADBC ).Return(curry(a)(b))));

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        public static MBC apply2<MONADABC, MONADA, MONADBC, MABC, MA, MBC, A, B, C>(MABC x, MA y)
            where MONADABC : struct, Monad<MABC, Func<A, Func<B, C>>>
            where MONADA   : struct, Monad<MA, A>
            where MONADBC  : struct, Monad<MBC, Func<B, C>> =>
            default(MONADABC).Bind<MONADBC, MBC, Func<B, C>>(x, a =>
            default(MONADA  ).Bind<MONADBC, MBC, Func<B, C>>(y, b =>
            default(MONADBC ).Return(a(b))));

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        public static MB action<MONADA, MONADB, MA, MB, A, B>(MA x, MB y)
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B> =>
            default(MONADA).Bind<MONADB, MB, B>(x, a =>
            default(MONADB).Bind<MONADB, MB, B>(y, b =>
            default(MONADB).Return(b)));
    }
}
