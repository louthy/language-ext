#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplEitherUnsafe<L, A, B> : 
        BiFunctor<EitherUnsafe<L, A>, EitherUnsafe<L, B>, L, A, L, B>,
        Applicative<EitherUnsafe<L, Func<A, B>>, EitherUnsafe<L, A>, EitherUnsafe<L, B>, A, B>
    {
        public static readonly ApplEitherUnsafe<L, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherUnsafe<L, B> Map(EitherUnsafe<L, A> ma, Func<A, B> f) =>
            default(FEitherUnsafe<L, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherUnsafe<L, B> Apply(EitherUnsafe<L, Func<A, B>> fab, EitherUnsafe<L, A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherUnsafe<L, B> Action(EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherUnsafe<L, A> Pure(A x) =>
            RightUnsafe<L, A>(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherUnsafe<L, B> BiMap(EitherUnsafe<L, A> ma, Func<L, L> Left, Func<A, B> Right) =>
            default(FEitherUnsafe<L, A, B>).BiMap(ma, Left, Right);
    }
}
