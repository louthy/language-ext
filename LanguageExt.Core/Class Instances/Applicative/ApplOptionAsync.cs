#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplOptionAsync<A, B> :
        ApplicativeAsync<OptionAsync<Func<A, B>>, OptionAsync<A>, OptionAsync<B>, A, B>
    {
        public static readonly ApplOptionAsync<A, B> Inst = default(ApplOptionAsync<A, B>);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            default(FOptionAsync<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, ValueTask<B>> f) =>
            default(FOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> Apply(OptionAsync<Func<A, B>> fab, OptionAsync<A> fa) =>
            fab.Apply(fa);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> Apply(Func<A, B> fab, OptionAsync<A> fa) =>
            fab.Apply(fa);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<A> PureAsync(Task<A> x) =>
            MOptionAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionAsync<B> Action(OptionAsync<A> fa, OptionAsync<B> fb) =>
            fa.Action(fb);
    }
}
