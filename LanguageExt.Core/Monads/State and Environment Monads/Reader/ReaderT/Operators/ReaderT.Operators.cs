using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ReaderT<Env, M, A> operator +(K<ReaderT<Env, M>, A> ma) =>
            (ReaderT<Env, M, A>)ma;
    }
}
