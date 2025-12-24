using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class PipeTExtensions
{
    extension<IN, OUT, M, A>(K<PipeT<IN, OUT, M>, A>)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static PipeT<IN, OUT, M, A> operator +(K<PipeT<IN, OUT, M>, A> ma) =>
           (PipeT<IN, OUT, M, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static PipeT<IN, OUT, M, A> operator >>(K<PipeT<IN, OUT, M>, A> ma, Lower lower) =>
            +ma;
    }
}
