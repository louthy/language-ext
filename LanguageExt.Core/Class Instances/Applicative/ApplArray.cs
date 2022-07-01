#nullable enable

using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplArray<A, B> : Applicative<Func<A, B>[], A[], B[], A, B>
    {
        public static readonly ApplArray<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B[] Action(A[] fa, B[] fb) =>
            (from a in fa
             from b in fb
             select b).ToArray();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B[] Apply(Func<A, B>[] fab, A[] fa) =>
            (from f in fab
             from a in fa
             select f(a)).ToArray();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B[] Map(A[] ma, Func<A, B> f) =>
            default(FArray<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public A[] Pure(A x) =>
            new [] { x };
    }
}
