using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static SourceT<M, A> operator +(K<SourceT<M>, A> ma) =>
            (SourceT<M, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static SourceT<M, A> operator >> (K<SourceT<M>, A> ma, Lower lower) =>
            +ma;
    }
}
