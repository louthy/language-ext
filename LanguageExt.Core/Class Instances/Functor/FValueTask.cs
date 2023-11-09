using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FValueTask<A, B> : 
        FunctorAsync<ValueTask<A>, ValueTask<B>, A, B>
    {
        public static readonly FValueTask<A, B> Inst = default(FValueTask<A, B>);

        [Pure]
        public async ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f) =>
            f(await ma.ConfigureAwait(false));

        [Pure]
        public async ValueTask<B> MapAsync(ValueTask<A> ma, Func<A, Task<B>> f) =>
            await f(await ma.ConfigureAwait(false)).ConfigureAwait(false);
    }
}
