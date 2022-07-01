#nullable enable

using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FEnumerable<A, B> :
        Functor<IEnumerable<A>, IEnumerable<B>, A, B>
    {
        public static readonly FEnumerable<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<B> Map(IEnumerable<A> ma, Func<A, B> f) =>
            ma.Select(f);
    }
}
