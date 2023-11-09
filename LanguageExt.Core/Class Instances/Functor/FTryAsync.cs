using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryAsync<A, B> : 
        FunctorAsync<TryAsync<A>, TryAsync<B>, A, B>
    {
        public static readonly FTryAsync<A, B> Inst = default(FTryAsync<A, B>);

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) =>
            default(MTryAsync<A>).Bind<MTryAsync<B>, TryAsync<B>, B>(ma, a => Prelude.TryAsync(f(a)));

        [Pure]
        public TryAsync<B> MapAsync(TryAsync<A> ma, Func<A, Task<B>> f) =>
            default(MTryAsync<A>).BindAsync<MTryAsync<B>, TryAsync<B>, B>(ma, async a => Prelude.TryAsync(await f(a).ConfigureAwait(false)));
    }
}
