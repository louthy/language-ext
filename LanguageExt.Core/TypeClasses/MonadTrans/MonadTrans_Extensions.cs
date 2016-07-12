using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Lifts the Option<A> into the monad transformer specified by TMA
        /// </summary>
        /// <typeparam name="TMA">Monad transformer to lift in to</typeparam>
        /// <typeparam name="A">Inner bound type</typeparam>
        /// <param name="ma">Monad to lift</param>
        /// <returns>Monad lifted into the transformer</returns>
        public static TMA Lift<TMA, A>(this Option<A> ma) 
            where TMA : struct, MonadT<Option<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

        /// <summary>
        /// Lifts the OptionUnsafe<A> into the monad transformer specified by TMA
        /// </summary>
        /// <typeparam name="TMA">Monad transformer to lift in to</typeparam>
        /// <typeparam name="A">Inner bound type</typeparam>
        /// <param name="ma">Monad to lift</param>
        /// <returns>Monad lifted into the transformer</returns>
        public static TMA Lift<TMA, A>(this OptionUnsafe<A> ma)
            where TMA : struct, MonadT<OptionUnsafe<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

        /// <summary>
        /// Lifts the Lst<A> into the monad transformer specified by TMA
        /// </summary>
        /// <typeparam name="TMA">Monad transformer to lift in to</typeparam>
        /// <typeparam name="A">Inner bound type</typeparam>
        /// <param name="ma">Monad to lift</param>
        /// <returns>Monad lifted into the transformer</returns>
        public static TMA Lift<TMA, A>(this Lst<A> ma)
            where TMA : struct, MonadT<Lst<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

        /// <summary>
        /// Lifts the Monad<A> into the monad transformer specified by TMA
        /// </summary>
        /// <typeparam name="TMA">Monad transformer to lift in to</typeparam>
        /// <typeparam name="A">Inner bound type</typeparam>
        /// <param name="ma">Monad to lift</param>
        /// <returns>Monad lifted into the transformer</returns>
        public static TMA Lift<TMA, A>(this Monad<A> ma)
            where TMA : struct, MonadT<Monad<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }



        private static OptionT<MA, A> OptionT<MA, A>(this MA ma) where MA : Monad<A> =>
            ma;

        private static void Test()
        {
            var x = List(1, 2, 3);

            var y = x.Lift<OptionT<Lst<int>, int>, int>();

            var z = (OptionT<Lst<int>, int>)x;

            OptionT<Lst<int>, int> c = x;

            var w = OptionT<Lst<int>, int>(x);
        }
    }
}
