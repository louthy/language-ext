using LanguageExt.Traits;

namespace LanguageExt;

public static class IndexableExtensions
{
    extension<T, KEY, VALUE>(K<T, VALUE> ta)
        where T : Indexable<T, KEY>
    {
        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        /// <param name="index">Index value</param>
        /// <returns>Result at index if found, otherwise `None`</returns>
        public Option<VALUE> At(KEY index) => 
            T.At(index, ta);
    }
}
