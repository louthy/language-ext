using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ConsumerTExtensions
{
    extension<IN, M, A>(K<ConsumerT<IN, M>, A>)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static ConsumerT<IN, M, A> operator +(K<ConsumerT<IN, M>, A> ma) =>
           (ConsumerT<IN, M, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static ConsumerT<IN, M, A> operator >>(K<ConsumerT<IN, M>, A> ma, Lower lower) =>
            +ma;
    }
}
