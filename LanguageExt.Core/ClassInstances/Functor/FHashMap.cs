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

    public struct FHashMap<EqK, K, A, B> :
        Functor<HashMap<EqK, K, A>, HashMap<EqK, K, B>, A, B>
        where EqK : struct, Eq<K>
    {
        public static readonly FHashMap<EqK, K, A, B> Inst = default(FHashMap<EqK, K, A, B>);

        [Pure]
        public HashMap<EqK, K, B> Map(HashMap<EqK, K, A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
