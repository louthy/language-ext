using System;
using System.Collections.Generic;
using System.Text;

using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class MonadTransAsyncExtensions
    {
        public static Task<int> CountAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.CountAsync(a);

        public static Task<bool> ForAllAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.FoldAsync(a, true, (s, x) => s && f(x));

        public static Task<bool> ExistsAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> m, OuterType a, Func<A, bool> f)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>.Inst.FoldAsync(a, false, (s, x) => s || f(x));

        public static Task<int> SumAsync<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, int> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, int> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TInt, int>.Inst.SumAsync(a);

        public static Task<float> SumAsync<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, float> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, float> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TFloat, float>.Inst.SumAsync(a);

        public static Task<double> SumAsync<OuterMonad, OuterType, InnerMonad, InnerType>(this MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, double> m, OuterType a)
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, double> =>
                TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, TDouble, double>.Inst.SumAsync(a);

    }
}