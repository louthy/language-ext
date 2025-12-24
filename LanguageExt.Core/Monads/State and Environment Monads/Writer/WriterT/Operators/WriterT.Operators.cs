using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, M, A>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static WriterT<W, M, A> operator +(K<WriterT<W, M>, A> ma) =>
            (WriterT<W, M, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static WriterT<W, M, A> operator >> (K<WriterT<W, M>, A> ma, Lower lower) =>
            +ma;
    }
}
