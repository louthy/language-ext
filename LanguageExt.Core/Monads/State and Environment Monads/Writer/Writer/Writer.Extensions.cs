using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterExtensions
{
    public static Writer<W, A> As<W, A>(this K<Writer<W>, A> ma)
        where W : Monoid<W> =>
        (Writer<W, A>)ma;

    /// <summary>
    /// Run the writer 
    /// </summary>
    /// <returns>Bound monad</returns>
    public static (A Value, W Output) Run<W, A>(this K<Writer<W>, A> ma, W initial)
        where W : Monoid<W> =>
        ((Writer<W, A>)ma).runWriter(initial);

    /// <summary>
    /// Run the writer 
    /// </summary>
    /// <returns>Bound monad</returns>
    public static (A Value, W Output) Run<W, A>(this K<Writer<W>, A> ma)
        where W : Monoid<W> =>
        ((Writer<W, A>)ma).runWriter(W.Empty);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Writer<W, A> Flatten<W, A>(this Writer<W, Writer<W, A>> mma)
        where W : Monoid<W> =>
        mma.Bind(x => x);
}
