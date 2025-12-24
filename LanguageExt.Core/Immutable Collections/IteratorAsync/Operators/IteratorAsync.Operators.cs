using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorAsyncExtensions
{
    extension<A>(K<IteratorAsync, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IteratorAsync<A> operator +(K<IteratorAsync, A> ma) =>
            (IteratorAsync<A>)ma;
        
        public static IteratorAsync<A> operator >> (K<IteratorAsync, A> ma, Lower lower) =>
            (IteratorAsync<A>)ma;
    }
}
