using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IdentityExtensions
{
    extension<A>(K<Identity, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Identity<A> operator +(K<Identity, A> ma) =>
            (Identity<A>)ma;
    }
}
