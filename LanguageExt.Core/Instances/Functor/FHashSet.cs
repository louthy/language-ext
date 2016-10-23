using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.Instances
{
    public struct FHashSet<A, B> : 
        Functor<HashSet<A>, HashSet<B>, A, B>
    {
        public HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
