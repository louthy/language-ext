namespace LanguageExt.Traits;

/// <summary>
/// Structure that supports element access by index.
/// </summary>
/// <remarks>
/// This is usually a hint that element access is fast and not dependent on the number of elements in the structure.
/// </remarks>
/// <typeparam name="T">Element value-type</typeparam>
/// <typeparam name="KEY">Index value-type</typeparam>
public interface Indexable<out T, in KEY>
    where T : Indexable<T, KEY>
{
    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static abstract Option<A> At<A>(KEY index, K<T, A> ta);
}
