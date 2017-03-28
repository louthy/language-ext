using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FSet<A, B> :
        Functor<Set<A>, Set<B>, A, B>
    {
        public static readonly FSet<A, B> Inst = default(FSet<A, B>);

        [Pure]
        public Set<B> Map(Set<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }

    public struct FSet<OrdA, OrdB, A, B> : Functor<Set<OrdA, A>, Set<OrdB, B>, A, B>
        where OrdA : struct, Ord<A>
        where OrdB : struct, Ord<B>
    {
        public static readonly FSet<OrdA, OrdB, A, B> Inst = default(FSet<OrdA, OrdB, A, B>);

        [Pure]
        public Set<OrdB, B> Map(Set<OrdA, A> ma, Func<A, B> f) =>
            ma.Map<OrdB, B>(f);
    }

}
