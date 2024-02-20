using System.Runtime.CompilerServices;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

public static class Enc<M, RT>
    where M : Reader<M, RT>, Monad<M>
    where RT : HasEncoding<RT>
{
    /// <summary>
    /// Encoding
    /// </summary>
    public static K<M, System.Text.Encoding> encoding
    {
        [MethodImpl(EffOpt.mops)] get => M.Asks(rt => rt.Encoding);
    }
}
