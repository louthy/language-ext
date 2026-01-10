namespace LanguageExt.Traits;

/// <summary>
/// Allows the acquisition of an iterator that iterates from end to beginning 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
public interface IterableBackK<F>
    where F : IterableBackK<F>
{
    static abstract Iterator<A> BackwardIterator<A>(K<F, A> fa);
}

/// <summary>
/// Allows the acquisition of an iterator that iterates from end to beginning 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
/// <typeparam name="FS">Low-level ref-struct state-type. Used to hold state for the duration of an iteration</typeparam>
public interface IterableBackK<F, FS> : IterableBackK<F>
    where F : IterableBackK<F, FS>
    where FS : allows ref struct
{
    static abstract FS StepBackSetup<A>(K<F, A> ta);
    static abstract bool StepBack<A>(K<F, A> ta, ref FS refState, out A value);
}
