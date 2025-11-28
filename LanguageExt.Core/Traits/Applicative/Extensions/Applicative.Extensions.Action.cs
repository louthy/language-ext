using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<F, A>(K<F, A> ma) where F : Applicative<F>
    {
        [Pure]
        public K<F, B> Action<B>(K<F, B> mb) =>
            F.Action(ma, mb);
    }

    [Pure]
    public static K<F, A> Actions<F, A>(this IEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
    
    [Pure]
    public static K<F, A> Actions<F, A>(this IAsyncEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
}
