using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomHashMapExtensions
{
    extension<Key, A>(K<AtomHashMap<Key>, A>)
    {
        public static AtomHashMap<Key, A> operator +(K<AtomHashMap<Key>, A> xs) =>
            (AtomHashMap<Key, A>)xs;

        public static AtomHashMap<Key, A> operator >> (K<AtomHashMap<Key>, A> lhs, Lower _) =>
            (AtomHashMap<Key, A>)lhs;
    }

    extension<EqKey, Key, A>(K<AtomHashMapEq<EqKey, Key>, A>)
        where EqKey : Eq<Key>
    {
        public static AtomHashMap<EqKey, Key, A> operator +(K<AtomHashMapEq<EqKey, Key>, A> xs) =>
            (AtomHashMap<EqKey, Key, A>)xs;
        
        public static AtomHashMap<EqKey, Key, A> operator >>(K<AtomHashMapEq<EqKey, Key>, A> xs, Lower _) =>
            (AtomHashMap<EqKey, Key, A>)xs;
    }
}
