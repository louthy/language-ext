namespace LanguageExt.Traits;

/// <summary>
/// Allows the acquisition of an iterator that iterates from beginning to end 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableK<F>
    where F : IterableK<F>
{
    static abstract Iterator<A> ForwardIterator<A>(K<F, A> fa);
}

/// <summary>
/// Allows the acquisition of an iterator that iterates from end to beginning 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableBackK<F>
    where F : IterableBackK<F>
{
    static abstract Iterator<A> BackwardIterator<A>(K<F, A> fa);
}
