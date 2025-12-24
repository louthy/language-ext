using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ProducerTExtensions
{
    extension<OUT, M, A>(K<ProducerT<OUT, M>, A>)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static ProducerT<OUT, M, A> operator +(K<ProducerT<OUT, M>, A> ma) =>
           (ProducerT<OUT, M, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static ProducerT<OUT, M, A> operator >>(K<ProducerT<OUT, M>, A> ma, Lower lower) =>
            +ma;
    }
}
