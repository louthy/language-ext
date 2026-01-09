using LanguageExt.Traits;

namespace LanguageExt;

public static class IterableKExtensions
{
    extension<F, A>(K<F, A> fa)
        where F : IterableK<F>
    {
        /// <summary>
        /// Get a forward iterator
        /// </summary>
        /// <returns></returns>
        public Iterator<A> ForwardIterator() =>
            F.ForwardIterator(fa);
    }
    
    extension<F, A>(K<F, A> fa)
        where F : IterableBackK<F>
    {
        /// <summary>
        /// Get a forward iterator
        /// </summary>
        /// <returns></returns>
        public Iterator<A> BackwardIterator() =>
            F.BackwardIterator(fa);
    }
}
