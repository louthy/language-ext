using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class RwsT
    {
        public static RWS<MonoidW, Env, W, S, Arr<B>> Sequence<MonoidW, Env, W, S, A, B>(this Arr<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f) 
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();

        public static RWS<MonoidW, Env, W, S, IEnumerable<B>> Sequence<MonoidW, Env, W, S, A, B>(this IEnumerable<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        public static RWS<MonoidW, Env, W, S, Set<B>> Sequence<MonoidW, Env, W, S, A, B>(this Set<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
    }
}
