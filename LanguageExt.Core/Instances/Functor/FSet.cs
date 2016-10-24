using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.Instances
{
    public struct FSet<A, B> : 
        Functor<Set<A>, Set<B>, A, B>
    {
        public static readonly FSet<A, B> Inst = default(FSet<A, B>);

        public Set<B> Map(Set<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
