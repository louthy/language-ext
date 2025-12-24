using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Eff<RT, A> operator +(K<Eff<RT>, A> ma) =>
            (Eff<RT, A>)ma;
        
        public static Eff<RT, A> operator >> (K<Eff<RT>, A> ma, Lower lower) =>
            (Eff<RT, A>)ma;
    }
}
