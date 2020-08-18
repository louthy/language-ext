using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTask<A, B> : 
        FunctorAsync<Task<A>, Task<B>, A, B>,
        BiFunctorAsync<Task<A>, Task<B>, A, Unit, B>
    {
        public static readonly FTask<A, B> Inst = default(FTask<A, B>);

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(MTask<A>).MatchAsync(ma, a => fa(a).AsTask(), () => fb(unit).AsTask());

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(MTask<A>).MatchAsync(ma, fa, () => fb(unit).AsTask());

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(MTask<A>).MatchAsync(ma, fa, () => fb(unit));

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(MTask<A>).MatchAsync(ma, fa, () => fb(unit));

        [Pure]
        public async Task<B> Map(Task<A> ma, Func<A, B> f) =>
            f(await ma.ConfigureAwait(false));

        [Pure]
        public async Task<B> MapAsync(Task<A> ma, Func<A, Task<B>> f) =>
            await f(await ma.ConfigureAwait(false)).ConfigureAwait(false);
    }
}
