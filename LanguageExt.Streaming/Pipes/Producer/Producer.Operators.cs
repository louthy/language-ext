using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ProducerExtensions
{
    extension<RT, OUT, A>(K<Producer<RT, OUT>, A>)
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static Producer<RT, OUT, A> operator +(K<Producer<RT, OUT>, A> ma) =>
           (Producer<RT, OUT, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static Producer<RT, OUT, A> operator >>(K<Producer<RT, OUT>, A> ma, Lower lower) =>
           +ma;
    }
}
