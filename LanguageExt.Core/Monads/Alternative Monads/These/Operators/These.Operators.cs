using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TheseExtensions
{
    extension<X, A>(K<These<X>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static These<X, A> operator +(K<These<X>, A> ma) =>
            (These<X, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static These<X, A> operator >> (K<These<X>, A> ma, Lower lower) =>
            +ma;
    }
}
