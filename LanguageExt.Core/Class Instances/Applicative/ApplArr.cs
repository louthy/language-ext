#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplArr<A, B> : Applicative<Arr<Func<A, B>>, Arr<A>, Arr<B>, A, B>
    {
        public static readonly ApplArr<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<B> Action(Arr<A> fa, Arr<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<B> Apply(Arr<Func<A, B>> fab, Arr<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<B> Map(Arr<A> ma, Func<A, B> f) =>
            default(FArr<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Pure(A x) =>
            Array(x);
    }
}
