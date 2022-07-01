#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FNullable<A, B> : 
        Functor<A?, B?, A, B>
        where A : struct
        where B : struct
    {
        public static readonly FNullable<A, B> Inst = default(FNullable<A, B>);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B? Map(A? ma, Func<A, B> f) =>
            ma.HasValue
                ? f(ma.Value)
                : null;
    }
}
