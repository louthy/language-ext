using LanguageExt.Traits;

namespace LanguageExt;

public static class IterableBackKExtensions
{
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
    
    extension<F, FS, A>(K<F, A> ta)
        where F : IterableBackK<F, FS>
        where FS : allows ref struct
    {
        /// <summary>
        /// Low-level interface for folding using stack-based primitives.
        /// </summary>
        public FS StepBackSetup() =>
            F.StepBackSetup(ta);

        /// <summary>
        /// Low-level interface for folding using stack-based primitives.
        /// </summary>
        public bool StepBack(ref FS refState, out A value) =>
            F.StepBack(ta, ref refState, out value);

        /// <summary>
        /// Create an iterator where all state is held on the stack
        /// </summary>
        public IterableBackEnumeratorRef<F, FS, A> BackwardIteratorRef() =>
            new (ta);
    }
}
