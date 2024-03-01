using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static class WriterExtensions
{
    public static Writer<W, A> As<W, A>(this K<Writer<W>, A> ma)
        where W : Monoid<W> =>
        (Writer<W, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Writer<W, A> Flatten<W, A>(this Writer<W, Writer<W, A>> mma)
        where W : Monoid<W> =>
        mma.Bind(x => x);
}
