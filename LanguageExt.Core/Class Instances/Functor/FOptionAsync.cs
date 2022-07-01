#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public readonly struct FOptionAsync<A, B> : FunctorAsync<OptionAsync<A>, OptionAsync<B>, A, B>
    {
        public static readonly FOptionAsync<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            new(ma.Effect.Map(f));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, ValueTask<B>> f) =>
            new(ma.Effect.MapAsync(f));
    }
}
