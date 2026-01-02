using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomQueExtensions
{
    extension<A>(K<AtomQue, A>)
    {
        public static AtomQue<A> operator +(K<AtomQue, A> xs) =>
            (AtomQue<A>)xs;

        public static AtomQue<A> operator >> (K<AtomQue, A> lhs, Lower _) =>
            (AtomQue<A>)lhs;
    }
}
