using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomSeqExtensions
{
    extension<A>(K<AtomSeq, A>)
    {
        public static AtomSeq<A> operator +(K<AtomSeq, A> xs) =>
            (AtomSeq<A>)xs;

        public static AtomSeq<A> operator >> (K<AtomSeq, A> lhs, Lower _) =>
            (AtomSeq<A>)lhs;
    }
}
