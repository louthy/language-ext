using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static RWST<R, W, S, M, A> operator +(K<RWST<R, W, S, M>, A> ma) =>
            (RWST<R, W, S, M, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static RWST<R, W, S, M, A> operator >> (K<RWST<R, W, S, M>, A> ma, Lower lower) =>
            +ma;
    }
}
