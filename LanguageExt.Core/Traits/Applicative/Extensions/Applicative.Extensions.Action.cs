using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

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
    public static K<F, A> Actions<F, A>(this IterableNE<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
}
