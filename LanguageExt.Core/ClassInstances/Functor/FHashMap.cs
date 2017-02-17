using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FHashMap<K, A, B> : 
        Functor<HashMap<K, A>, HashMap<K, B>, A, B>
    {
        public static readonly FHashMap<K, A, B> Inst = default(FHashMap<K, A, B>);

        [Pure]
        public HashMap<K, B> Map(HashMap<K, A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
