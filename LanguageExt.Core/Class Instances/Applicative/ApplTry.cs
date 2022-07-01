#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplTry<A, B> : 
        BiFunctor<Try<A>, Try<B>, Error, A, Error, B>,
        Applicative<Try<Func<A, B>>, Try<A>, Try<B>, A, B>
    {
        public static readonly ApplTry<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> Apply(Try<Func<A, B>> fab, Try<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<A> Pure(A x) =>
            () => x;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> Action(Try<A> fa, Try<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> BiMap(Try<A> ma, Func<Error, Error> fa, Func<A, B> fb) => 
            default(FTry<A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> Map(Try<A> ma, Func<A, B> f) => 
            default(FTry<A, B>).Map(ma, f);
    }
}
