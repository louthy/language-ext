using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        ///// <summary>
        ///// Construct an applicative from an A
        ///// </summary>
        ///// <typeparam name="MA">Type of applicative to construct</typeparam>
        ///// <typeparam name="A">Type of the applicative value</typeparam>
        ///// <param name="a">Applicative value</param>
        ///// <returns>Applicative of A</returns>
        //[Pure]
        //public static MA Pure<ApplicativeA, MA, A>(A x, params A[] xs) where ApplicativeA : struct, ApplicativePure<MA, A> =>
        //    default(ApplicativeA).Pure(x);

        ///// <summary>
        ///// Sequential application
        ///// 
        /////     f(a -> b) -> f a -> f b
        ///// </summary>
        //[Pure]
        //public static MB apply<ApplicativeAB, ApplicativeA, ApplicativeB, MAB, MA, MB, A, B>(MAB x, MA y)
        //    where ApplicativeAB : struct, Applicative<MAB, Func<A, B>>
        //    where ApplicativeA  : struct, Applicative<MA, A>
        //    where ApplicativeB  : struct, Applicative<MB, B> =>
        //    default(ApplicativeAB).Bind<ApplicativeB, MB, B>(x, a =>
        //    default(ApplicativeA).Bind<ApplicativeB, MB, B>(y, b =>
        //    default(ApplicativeB).Return(a(b))));

        ///// <summary>
        ///// Sequential application 
        ///// 
        /////     f(a -> b -> c) -> f a -> f b -> f c
        ///// </summary>
        //[Pure]
        //public static MC apply<ApplicativeABC, ApplicativeA, ApplicativeB, ApplicativeC, MABC, MA, MB, MC, A, B, C>(MABC x, MA y, MB z)
        //    where ApplicativeABC : struct, Applicative<MABC, Func<A, B, C>>
        //    where ApplicativeA : struct, Applicative<MA, A>
        //    where ApplicativeB : struct, Applicative<MB, B>
        //    where ApplicativeC : struct, Applicative<MC, C> =>
        //    default(ApplicativeABC).Bind<ApplicativeC, MC, C>(x, a =>
        //    default(ApplicativeA).Bind<ApplicativeC, MC, C>(y, b =>
        //    default(ApplicativeB).Bind<ApplicativeC, MC, C>(z, c =>
        //    default(ApplicativeC).Return(a(b, c)))));

        ///// <summary>
        ///// Sequential application 
        ///// 
        /////     f(a -> b -> c) -> f a -> f b -> f c
        ///// </summary>
        //[Pure]
        //public static MBC apply<ApplicativeABC, ApplicativeA, ApplicativeBC, MABC, MA, MBC, A, B, C>(MABC x, MA y)
        //    where ApplicativeABC : struct, Applicative<MABC, Func<A, B, C>>
        //    where ApplicativeA : struct, Applicative<MA, A>
        //    where ApplicativeBC : struct, Applicative<MBC, Func<B, C>> =>
        //    default(ApplicativeABC).Bind<ApplicativeBC, MBC, Func<B,C>>(x, a =>
        //    default(ApplicativeA).Bind<ApplicativeBC, MBC, Func<B, C>>(y, b =>
        //    default(ApplicativeBC).Return(curry(a)(b))));

        ///// <summary>
        ///// Sequential application
        ///// 
        /////     f(a -> b -> c) -> f a -> f(b -> c)
        ///// </summary>
        //[Pure]
        //public static MBC apply2<ApplicativeABC, ApplicativeA, ApplicativeBC, MABC, MA, MBC, A, B, C>(MABC x, MA y)
        //    where ApplicativeABC : struct, Applicative<MABC, Func<A, Func<B, C>>>
        //    where MONADA   : struct, Applicative<MA, A>
        //    where ApplicativeBC : struct, Applicative<MBC, Func<B, C>> =>
        //    default(ApplicativeABC).Bind<ApplicativeBC, MBC, Func<B, C>>(x, a =>
        //    default(ApplicativeA).Bind<ApplicativeBC, MBC, Func<B, C>>(y, b =>
        //    default(ApplicativeBC).Return(a(b))));

        ///// <summary>
        ///// Sequential actions
        ///// 
        /////     f a -> f b -> f b
        ///// </summary>
        //[Pure]
        //public static MB action<ApplicativeA, ApplicativeB, MA, MB, A, B>(MA x, MB y)
        //    where ApplicativeA : struct, Applicative<MA, A>
        //    where ApplicativeB : struct, Applicative<MB, B> =>
        //    default(ApplicativeA).Bind<ApplicativeB, MB, B>(x, a =>
        //    default(ApplicativeB).Bind<ApplicativeB, MB, B>(y, b =>
        //    default(ApplicativeB).Return(b)));
    }
}
