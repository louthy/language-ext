using LanguageExt.Traits;

namespace LanguageExt;

public static class IterableBackKIOExtensions
{
    extension<F, A>(K<F, A> fa)
        where F : IterableBackKIO<F>
    {
        /// <summary>
        /// Get a forward iterator
        /// </summary>
        /// <returns></returns>
        public IteratorIO<A> BackwardIteratorIO() =>
            F.BackwardIteratorIO(fa);
    }
}
