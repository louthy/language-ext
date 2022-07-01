#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public struct FLst<A, B> :
        Functor<Lst<A>, Lst<B>, A, B>
    {
        public static readonly FLst<A, B> Inst = default(FLst<A, B>);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<B> Map(Lst<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
