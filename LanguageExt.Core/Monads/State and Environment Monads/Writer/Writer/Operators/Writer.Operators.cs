using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterExtensions
{
    extension<W, A>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Writer<W, A> operator +(K<Writer<W>, A> ma) =>
            (Writer<W, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Writer<W, A> operator >> (K<Writer<W>, A> ma, Lower lower) =>
            +ma;
    }
}
