using LanguageExt.Traits;

namespace LanguageExt;

public static partial class AtomQueExtensions
{
    public static AtomQue<A> As<A>(this K<AtomQue, A> ma) =>
        (AtomQue<A>)ma;
}
