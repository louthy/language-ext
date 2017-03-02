using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class MonadTransExt
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
    }
}
