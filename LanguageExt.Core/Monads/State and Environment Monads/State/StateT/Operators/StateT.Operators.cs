using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, M, A>(K<StateT<S, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static StateT<S, M, A> operator +(K<StateT<S, M>, A> ma) =>
            (StateT<S, M, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static StateT<S, M, A> operator >> (K<StateT<S, M>, A> ma, Lower lower) =>
            +ma;
    }
}
