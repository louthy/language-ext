using LanguageExt.Traits;

namespace LanguageExt;

public static class IterableKIOExtensions
{
    extension<F, A>(K<F, A> fa)
        where F : IterableKIO<F>
    {
        /// <summary>
        /// Get a forward iterator
        /// </summary>
        /// <returns></returns>
        public IteratorIO<A> ForwardIteratorIO() =>
            F.ForwardIteratorIO(fa);
    }
}
