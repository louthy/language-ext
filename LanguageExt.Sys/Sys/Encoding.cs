using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

public static class Enc<M, RT>
    where M : State<M, RT>, Monad<M>
    where RT : Has<M, EncodingIO>
{
    static readonly K<M, EncodingIO> trait = 
        State.getsM<M, RT, EncodingIO>(e => e.Trait);

    /// <summary>
    /// Encoding
    /// </summary>
    public static K<M, System.Text.Encoding> encoding =>
        trait.Bind(e => e.Encoding);
}
