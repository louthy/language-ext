using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FHashSet<A, B> : 
        Functor<HashSet<A>, HashSet<B>, A, B>
    {
        public static readonly FHashSet<A, B> Inst = default(FHashSet<A, B>);

        [Pure]
        public HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }

    public struct FHashSet<EqA, EqB, A, B> :
        Functor<HashSet<EqA, A>, HashSet<EqB, B>, A, B>
        where EqA : struct, Eq<A>
        where EqB : struct, Eq<B>
    {
        public static readonly FHashSet<EqA, EqB, A, B> Inst = default(FHashSet<EqA, EqB, A, B>);

        [Pure]
        public HashSet<EqB, B> Map(HashSet<EqA, A> ma, Func<A, B> f) =>
            ma.Map<EqB, B>(f);
    }
}
