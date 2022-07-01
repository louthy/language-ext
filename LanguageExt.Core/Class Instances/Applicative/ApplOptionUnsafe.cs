#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplOptionUnsafe<A, B> : Applicative<OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, A, B>
    {
        public static readonly ApplOptionUnsafe<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionUnsafe<B> Map(OptionUnsafe<A> ma, Func<A, B> f) =>
            default(FOptionUnsafe<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionUnsafe<B> Apply(OptionUnsafe<Func<A, B>> fab, OptionUnsafe<A> fa) =>
            (fab, fa) switch
            {
                #nullable disable
                ({IsSome: true, Value: not null} f, {IsSome: true} a) => f.Value(a.Value),
                #nullable enable
                _ => OptionUnsafe<B>.None 
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionUnsafe<A> Pure(A x) =>
            x;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionUnsafe<B> Action(OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
            fb;
    }
}
