using LanguageExt.Traits;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    extension<K, A>(K<HashMap<K>, A>)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static HashMap<K, A> operator +(K<HashMap<K>, A> ma) =>
            (HashMap<K, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static HashMap<K, A> operator >> (K<HashMap<K>, A> ma, Lower lower) =>
            (HashMap<K, A>)ma;
    }
}
