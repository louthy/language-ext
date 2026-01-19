namespace LanguageExt.Traits;

/// <summary>
/// Allows the acquisition of an iterator that iterates from end to beginning 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableBackKIO<F>
    where F : IterableBackKIO<F>
{
    static abstract IteratorIO<A> BackwardIteratorIO<A>(K<F, A> fa);
}
