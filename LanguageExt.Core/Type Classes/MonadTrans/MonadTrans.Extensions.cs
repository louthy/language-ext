using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static class MonadTransExtensions
    {
        public static int Count<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, A> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.Count(a);

        public static bool ForAll<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, A> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.Fold(a, true, (s, x) => s && f(x));

        public static bool Exists<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, A> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.Fold(a, false, (s, x) => s || f(x));

        public static int Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, int> m, OuterType a)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, int> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, TInt, int>.Inst.Sum(a);

        public static float Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, float> m, OuterType a)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, float> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, TFloat, float>.Inst.Sum(a);

        public static double Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, double> m, OuterType a)
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, double> =>
                Trans<OuterMonad, OuterType, InnerMonad, InnerType, TDouble, double>.Inst.Sum(a);

    }
}