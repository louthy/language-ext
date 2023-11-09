using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryOptionAsync<A, B> : 
        FunctorAsync<TryOptionAsync<A>, TryOptionAsync<B>, A, B>
    {
        public static readonly FTryOptionAsync<A, B> Inst = default(FTryOptionAsync<A, B>);

        [Pure]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            default(MTryOptionAsync<A>).Bind<MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, a => Prelude.TryOptionAsync(f(a)));

        [Pure]
        public TryOptionAsync<B> MapAsync(TryOptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(MTryOptionAsync<A>).BindAsync<MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, async a => Prelude.TryOptionAsync(await f(a)));
    }
}
