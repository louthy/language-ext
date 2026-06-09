using LanguageExt.Traits;

namespace LanguageExt;

public static class Indexable
{
    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    /// <param name="index">Index value</param>
    /// <returns>Result at index if found, otherwise `None`</returns>
    public static Option<VALUE> at<T, KEY, VALUE>(KEY index, K<T, VALUE> ta) 
        where T : Indexable<T, KEY> => 
        T.At(index, ta);
}
