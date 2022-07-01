#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplOption<A, B> : Applicative<Option<Func<A, B>>, Option<A>, Option<B>, A, B>
    {
        public static readonly ApplOption<A, B> Inst = default(ApplOption<A, B>);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<B> Map(Option<A> ma, Func<A, B> f) =>
            default(FOption<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<B> Apply(Option<Func<A, B>> fab, Option<A> fa) =>
            (fab, fa) switch
            {
                #nullable disable
                ({IsSome: true, Value: not null} f, {IsSome: true, Value: not null} a) => f.Value(a.Value),
                #nullable enable
                _ => Option<B>.None 
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<A> Pure(A x) =>
            x;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<B> Action(Option<A> fa, Option<B> fb) =>
            fb;
    }
}
