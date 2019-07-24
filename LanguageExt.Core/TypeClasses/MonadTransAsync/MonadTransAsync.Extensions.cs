using System;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class MonadTransAsyncExtensions
    {
        public static Task<int> Count<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.CountAsync(a);

        public static Task<bool> ForAll<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.Fold(a, true, (s, x) => s && f(x));

        public static Task<bool> Exists<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.Fold(a, false, (s, x) => s || f(x));

        public static Task<int> Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, int> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, int> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TInt, int>.Inst.Sum(a);

        public static Task<float> Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, float> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, float> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TFloat, float>.Inst.Sum(a);

        public static Task<double> Sum<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, double> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, double> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TDouble, double>.Inst.Sum(a);

    }
}