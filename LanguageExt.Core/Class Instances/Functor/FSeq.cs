#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FSeq<A, B> : Functor<Seq<A>, Seq<B>, A, B>
    {
        public static readonly FSeq<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
