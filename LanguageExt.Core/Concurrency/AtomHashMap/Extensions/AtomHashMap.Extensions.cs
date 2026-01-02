using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomHashMapExtensions
{
    extension<Key, A>(K<AtomHashMap<Key>, A> self)
    {
        public AtomHashMap<Key, A> As() =>
            (AtomHashMap<Key, A>)self;
    }

    extension<EqKey, Key, A>(K<AtomHashMapEq<EqKey, Key>, A> self)
        where EqKey : Eq<Key>
    {
        public AtomHashMap<EqKey, Key, A> As() =>
            (AtomHashMap<EqKey, Key, A>)self;
    }
}
