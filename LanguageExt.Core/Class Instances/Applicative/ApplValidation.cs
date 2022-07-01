#nullable enable

using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplValidation<MonoidFAIL, FAIL, A, B> : 
        BiFunctor<Validation<MonoidFAIL, FAIL, A>, Validation<MonoidFAIL, FAIL, B>, FAIL, A, FAIL, B>,
        Applicative<Validation<MonoidFAIL, FAIL, Func<A, B>>, Validation<MonoidFAIL, FAIL, A>, Validation<MonoidFAIL, FAIL, B>, A, B>
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL>
    {
        public static readonly ApplValidation<MonoidFAIL, FAIL, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFAIL, FAIL, B> BiMap(Validation<MonoidFAIL, FAIL, A> ma, Func<FAIL, FAIL> fa, Func<A, B> fb) =>
            default(FValidation<MonoidFAIL, FAIL, A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFAIL, FAIL, B> Map(Validation<MonoidFAIL, FAIL, A> ma, Func<A, B> f) =>
            default(FValidation<MonoidFAIL, FAIL, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFAIL, FAIL, B> Apply(Validation<MonoidFAIL, FAIL, Func<A, B>> fab, Validation<MonoidFAIL, FAIL, A> fa) =>
            (fab, fa).Apply((f, a) => f(a));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFAIL, FAIL, B> Action(Validation<MonoidFAIL, FAIL, A> fa, Validation<MonoidFAIL, FAIL, B> fb) =>
            (fa, fb).Apply((_, b) => b);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFAIL, FAIL, A> Pure(A x) =>
            Success<MonoidFAIL, FAIL, A>(x);
    }
}
