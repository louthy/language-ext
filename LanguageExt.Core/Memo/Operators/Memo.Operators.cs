using LanguageExt.Traits;

namespace LanguageExt;

public static partial class MemoExtensions
{
    extension<F, A>(Memo<F, A> self)
    {
        /// <summary>
        /// Extract the cached value
        /// </summary>
        /// <param name="ma">Memoisation structure</param>
        /// <returns>Cached value</returns>
        public static K<F, A> operator~(Memo<F, A> ma) =>
            ma.Value;
        
    }

    extension<A>(Memo<A> self)
    {
        /// <summary>
        /// Extract the cached value
        /// </summary>
        /// <param name="ma">Memoisation structure</param>
        /// <returns>Cached value</returns>
        public static A operator~(Memo<A> ma) =>
            ma.Value;
    }
}
