using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static TryT<M, A> operator +(K<TryT<M>, A> ma) =>
            (TryT<M, A>)ma;
    }
}
