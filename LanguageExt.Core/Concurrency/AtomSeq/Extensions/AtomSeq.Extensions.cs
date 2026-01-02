using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomSeqExtensions
{
    public static AtomSeq<A> As<A>(this K<AtomSeq, A> ma) =>
        (AtomSeq<A>)ma;
}
