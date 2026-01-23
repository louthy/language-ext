namespace LanguageExt.Traits;

/// <summary>
/// Allows the acquisition of an iterator that iterates from beginning to end 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableK<F>// : Natural<F, Iterator>
    where F : IterableK<F>
{
    /// <summary>
    /// Returns an iterator that iterates from beginning to end
    /// </summary>
    /// <param name="fa">Iterable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator</returns>
    static abstract Iterator<A> ForwardIterator<A>(K<F, A> fa);
}

/// <summary>
/// Allows the acquisition of an iterator that iterates from beginning to end 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
/// <typeparam name="FS">Low-level ref-struct state-type. Used to hold state for the duration of an iteration</typeparam>
public interface IterableK<F, FS> : IterableK<F>
    where F : IterableK<F, FS>
    where FS : allows ref struct
{
    /// <summary>
    /// Low-level interface for iterating using stack-based primitives.  This sets up a state-value.
    /// </summary>
    /// <param name="ta">Iterable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>The initial state for the iteration</returns>
    static abstract FS StepSetup<A>(K<F, A> ta);
    
    /// <summary>
    /// Low-level interface for stepping through the iterator.  This takes an `FS` state-value and returns
    /// the next value in the sequence, along with a new `FS` state-value.
    /// </summary>
    /// <param name="ta">Iterable structure</param>
    /// <param name="refState">The state that allows the iteration to be managed efficiently</param>
    /// <param name="value">The next value in the iterable structure (if the result is `true`)</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>True if the next value was retrieved, false if the iteration is complete</returns>
    static abstract bool Step<A>(K<F, A> ta, ref FS refState, out A value);
}
