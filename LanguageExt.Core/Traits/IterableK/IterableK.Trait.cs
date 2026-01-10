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
/// Allows the acquisition of an iterator that iterates from beginning to end 
/// </summary>
/// <typeparam name="F">Trait implementation type</typeparam>
/// <typeparam name="FS">Low-level ref-struct state-type. Used to hold state for the duration of an iteration</typeparam>
public interface IterableK<F, FS> : IterableK<F>
    where F : IterableK<F, FS>
    where FS : allows ref struct
{
    static abstract FS StepSetup<A>(K<F, A> ta);
    static abstract bool Step<A>(K<F, A> ta, ref FS refState, out A value);
}
