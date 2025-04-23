using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ConduitExtensions
{
    public static Conduit<A, B> As<A, B>(this K<Conduit<A>, B> ma) => 
        (Conduit<A, B>)ma;
}
