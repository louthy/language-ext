using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<F, A>(K<Free<F>, A> _)
        where F : Functor<F>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Free<F, A> operator +(K<Free<F>, A> ma) =>
            (Free<F, A>)ma;
    }
}
