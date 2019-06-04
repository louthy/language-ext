using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static class MonadTransExtensions
    {
        public static int Count<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> m, OuterType a)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>.Inst.Count(a);

        public static bool ForAll<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>.Inst.Fold(a, true, (s, x) => s && f(x));

        public static bool Exists<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>.Inst.Fold(a, false, (s, x) => s || f(x));

        public static int Sum<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, int> m, OuterType a)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, int> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, TInt, int>.Inst.Sum(a);

        public static float Sum<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, float> m, OuterType a)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, float> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, TFloat, float>.Inst.Sum(a);

        public static double Sum<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType>(this MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, double> m, OuterType a)
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, double> =>
                Trans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, TDouble, double>.Inst.Sum(a);

    }
}