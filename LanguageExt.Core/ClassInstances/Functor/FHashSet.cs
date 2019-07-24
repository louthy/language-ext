using System;
using System.Linq;
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
}
