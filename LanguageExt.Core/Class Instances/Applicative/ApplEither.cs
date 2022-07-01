#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplEither<L, A, B> : 
        BiFunctor<Either<L, A>, Either<L, B>, L, A, L, B>,
        Applicative<Either<L, Func<A, B>>, Either<L, A>, Either<L, B>, A, B>
    {
        public static readonly ApplEither<L, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, B> Map(Either<L, A> ma, Func<A, B> f) =>
            default(FEither<L, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, B> Apply(Either<L, Func<A, B>> fab, Either<L, A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, B> Action(Either<L, A> fa, Either<L, B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, A> Pure(A x) =>
            Right<L, A>(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, B> BiMap(Either<L, A> ma, Func<L, L> Left, Func<A, B> Right) =>
            default(FEither<L, A, B>).BiMap(ma, Left, Right);
    }
}
