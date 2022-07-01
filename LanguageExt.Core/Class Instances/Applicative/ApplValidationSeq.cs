#nullable enable

using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplValidation<FAIL, A, B> : 
        BiFunctor<Validation<FAIL, A>, Validation<FAIL, B>, Seq<FAIL>, A, Seq<FAIL>, B>,
        Applicative<Validation<FAIL, Func<A, B>>, Validation<FAIL, A>, Validation<FAIL, B>, A, B>
    {
        public static readonly ApplValidation<FAIL, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> BiMap(Validation<FAIL, A> ma, Func<Seq<FAIL>, Seq<FAIL>> fa, Func<A, B> fb) =>
            default(FValidation<FAIL, A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> Map(Validation<FAIL, A> ma, Func<A, B> f) =>
            default(FValidation<FAIL, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> Apply(Validation<FAIL, Func<A, B>> fab, Validation<FAIL, A> fa) =>
            (fab, fa).Apply((f, a) => f(a));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> Action(Validation<FAIL, A> fa, Validation<FAIL, B> fb) =>
            (fa, fb).Apply((_, b) => b);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, A> Pure(A x) =>
            Success<FAIL, A>(x);
    }
}
