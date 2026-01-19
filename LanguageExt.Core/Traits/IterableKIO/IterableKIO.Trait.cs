namespace LanguageExt.Traits;

/// <summary>
/// Allows the acquisition of an iterator that iterates from beginning to end 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableKIO<F>
    where F : IterableKIO<F>
{
    /// <summary>
    /// Returns an iterator that iterates from beginning to end
    /// </summary>
    /// <param name="fa">Iterable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator</returns>
    static abstract IteratorIO<A> ForwardIteratorIO<A>(K<F, A> fa);
}
