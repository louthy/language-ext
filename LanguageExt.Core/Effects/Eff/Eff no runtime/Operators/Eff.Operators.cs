using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<A>(K<Eff, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Eff<A> operator +(K<Eff, A> ma) =>
            (Eff<A>)ma;
    }
}
