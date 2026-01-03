using LanguageExt.Traits;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    extension<EqK, K, A>(K<HashMapEq<EqK, K>, A>)
        where EqK : Eq<K>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static HashMap<EqK, K, A> operator +(K<HashMapEq<EqK, K>, A> ma) =>
            (HashMap<EqK, K, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static HashMap<EqK, K, A> operator >> (K<HashMapEq<EqK, K>, A> ma, Lower lower) =>
            (HashMap<EqK, K, A>)ma;
    }
}
