using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.Instances
{
    public struct FHashMap<K, A, B> : 
        Functor<HashMap<K, A>, HashMap<K, B>, A, B>
    {
        public HashMap<K, B> Map(HashMap<K, A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
