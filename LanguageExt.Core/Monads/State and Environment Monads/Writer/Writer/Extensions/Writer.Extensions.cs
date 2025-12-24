using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterExtensions
{
    extension<W, A>(K<Writer<W>, A> ma) where W : Monoid<W>
    {
        public Writer<W, A> As() =>
            (Writer<W, A>)ma;

        /// <summary>
        /// Run the writer 
        /// </summary>
        /// <returns>Bound monad</returns>
        public (A Value, W Output) Run(W initial) =>
            ((Writer<W, A>)ma).runWriter(initial);

        /// <summary>
        /// Run the writer 
        /// </summary>
        /// <returns>Bound monad</returns>
        public (A Value, W Output) Run() =>
            ((Writer<W, A>)ma).runWriter(W.Empty);
    }

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Writer<W, A> Flatten<W, A>(this Writer<W, Writer<W, A>> mma)
        where W : Monoid<W> =>
        mma.Bind(x => x);
}
