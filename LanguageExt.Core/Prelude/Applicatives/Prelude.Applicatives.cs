using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// </summary>
    /// <remarks>
    /// Runs each applicative and returns the result of the last one
    /// </remarks>
    public static K<F, A> actions<F, A>(params K<F, A>[] ms)
        where F : Applicative<F> =>
        F.Actions(ms);

    /// <summary>
    /// Applicative actions
    /// </summary>
    /// <remarks>
    /// Runs each applicative and returns the result of the last one
    /// </remarks>
    public static K<F, A> actions<F, A>(IEnumerable<K<F, A>> ms)
        where F : Applicative<F> =>
        F.Actions(ms);

    /// <summary>
    /// Applicative actions
    /// </summary>
    /// <remarks>
    /// Runs each applicative and returns the result of the last one
    /// </remarks>
    public static K<F, A> actions<F, A>(Seq<K<F, A>> ms)
        where F : Applicative<F> =>
        F.Actions(ms);
}
