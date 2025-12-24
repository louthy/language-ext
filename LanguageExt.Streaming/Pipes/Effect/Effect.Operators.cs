using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class EffectExtensions
{
    extension<RT, A>(K<Effect<RT>, A>)
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static Effect<RT, A> operator +(K<Effect<RT>, A> ma) =>
           (Effect<RT, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static Effect<RT, A> operator >>(K<Effect<RT>, A> ma, Lower lower) =>
            +ma;
    }
}
