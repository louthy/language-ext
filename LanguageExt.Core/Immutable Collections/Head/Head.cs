namespace LanguageExt;

/// <summary>
/// Used for pattern-matching the existence of a head-value in a sequence, namely `Iterator`.
/// This is like `Option` but in reference-type form to make it easier to pattern-match against.
/// </summary>
/// <typeparam name="A">Value type</typeparam>
public abstract record Head<A>
{
    /// <summary>
    /// An existent value
    /// </summary>
    /// <param name="value">The value that exists</param>
    /// <returns>Head in an 'exists' state</returns>
    public static Head<A> Exist(A value) => new Exist<A>(value);

    /// <summary>
    /// A non-existent value
    /// </summary>
    /// <returns>Head in a 'non-exists' state</returns>
    public static Head<A> Nil => Nil<A>.Default; 
    
    /// <summary>
    /// Protected constructor to stop others from subtyping 
    /// </summary>
    protected Head() { }
}
