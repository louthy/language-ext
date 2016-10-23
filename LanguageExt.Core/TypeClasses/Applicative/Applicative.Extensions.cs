using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        public static MB Apply<MONADAB, MONADA, MONADB, MAB, MA, MB, A, B>(this MAB x, MA y)
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
        public static MC Apply<MONADABC, MONADA, MONADB, MONADC, MABC, MA, MB, MC, A, B, C>(this MABC x, MA y, MB z)
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
        public static MB Action<MONADA, MONADB, MA, MB, A, B>(this MA x, MB y)
            where MONADA : struct, Monad<MA, A>
            where MONADB : struct, Monad<MB, B> =>
            default(MONADA).Bind<MONADB, MB, B>(x, a =>
            default(MONADB).Bind<MONADB, MB, B>(y, b =>
            default(MONADB).Return(b)));
    }
}
