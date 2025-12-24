using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class EffectTExtensions
{
    extension<M, A>(K<EffectT<M>, A>)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static EffectT<M, A> operator +(K<EffectT<M>, A> ma) =>
           (EffectT<M, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static EffectT<M, A> operator >>(K<EffectT<M>, A> ma, Lower lower) =>
            +ma;
    }
}
