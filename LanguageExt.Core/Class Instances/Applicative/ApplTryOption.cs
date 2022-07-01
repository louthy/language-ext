#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplTryOption<A, B> : 
        BiFunctor<TryOption<A>, TryOption<B>, Error, A, Error, B>,
        Applicative<TryOption<Func<A, B>>, TryOption<A>, TryOption<B>, A, B>
    {
        public static readonly ApplTryOption<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> Apply(TryOption<Func<A, B>> fab, TryOption<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<A> Pure(A x) =>
            default(MTryOption<A>).Return(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> Action(TryOption<A> fa, TryOption<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> BiMap(TryOption<A> ma, Func<Error, Error> fa, Func<A, B> fb) => 
            default(FTryOption<A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> Map(TryOption<A> ma, Func<A, B> f) => 
            default(FTryOption<A, B>).Map(ma, f);
    }
}
